namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

/// <summary>
/// 用户SDK管理接口
/// </summary>
[ApiController]
[Route("api/v1")]
public class GameSDKForPlayerController : ControllerBase
{
    private const int _pageSize = 10;
    private readonly ILogger<GameSDKForPlayerController> _logger;
    private readonly IGameOwnerService _ownerService;
    private readonly ISDKForPlayerService _sdkForPlayerService;
    private readonly IGameItemSDKService _sdkSendService;
    private readonly IGameInfoService _gameInfoService;
    private readonly IUnitOfWorkService _unitOfWorkService;

    public GameSDKForPlayerController(
        ISDKForPlayerService sdkForPlayerService,
        ILogger<GameSDKForPlayerController> logger,
        IGameItemSDKService sdkSendService,
        IGameOwnerService ownerService,
        IUnitOfWorkService unitOfWorkService,
        IGameInfoService gameInfoService)
    {
        _sdkForPlayerService = sdkForPlayerService ?? throw new ArgumentNullException(nameof(sdkForPlayerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _sdkSendService = sdkSendService ?? throw new ArgumentNullException(nameof(sdkSendService));
        _ownerService = ownerService ?? throw new ArgumentNullException(nameof(ownerService));
        _unitOfWorkService = unitOfWorkService ?? throw new ArgumentNullException(nameof(unitOfWorkService));
        _gameInfoService = gameInfoService ?? throw new ArgumentNullException(nameof(gameInfoService));
    }

    /// <summary>
    /// 用户——分页获取自己购买的游戏SDK列表
    /// </summary>
    /// <param name="hasChecked">是否已使用</param>
    /// <param name="pageIndex">pageSize=10</param>
    /// <returns></returns>
    [HttpGet("game/user/sdks")]
    [Authorize]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<PlaySDKDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserSDKsByUserIdAsync([FromQuery] bool? hasChecked, [FromQuery] int pageIndex = 1)
    {
        var userId = User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId)) return BadRequest();

        var userSdkCount = await _sdkForPlayerService.CountPlayerSDKByUserId(userId);
        if (!ParameterValidateHelper.IsInvalidPageIndex(userSdkCount, pageIndex, _pageSize)) pageIndex = 1;

        // 筛选查询
        var userSdksDto = hasChecked switch
        {
            false => await _sdkForPlayerService.GetPlayerSDKByUserIdAndStatusAsync(userId, _pageSize, pageIndex, null),
            true => await _sdkForPlayerService.GetPlayerSDKByUserIdAndStatusAsync(userId, _pageSize, pageIndex, true),
            _ => await _sdkForPlayerService.GetPlayerSDKByUserIdAsync(userId, _pageSize, pageIndex)
        };

        _logger.LogInformation("user:{UserId}-{UserName}, get it sdkList", userId, User.Identity.Name);
        var model = new PaginatedItemsDtoModel<PlaySDKDto>(pageIndex, _pageSize, userSdkCount, userSdksDto);
        return Ok(model);
    }

    /// <summary>
    /// 用户——校验并使用自己的游戏SDK
    /// </summary>
    /// <param name="sdkId"></param>
    /// <returns></returns>
    [HttpPut("game/user/sdk/{sdkId:int}")]
    [Authorize]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CheckUserSdkAsync([FromRoute] int sdkId)
    {
        if (sdkId <= 0 || sdkId >= int.MaxValue) return BadRequest();
        var userId = User.FindFirstValue("sub");

        var sdk = await _sdkForPlayerService.GetPlayerSDKByIdAsync(sdkId);
        if (sdk.UserId != userId) return BadRequest();

        _logger.LogInformation("user:{UserId}-{UserName} start to check itself sdk:{SDKId}", userId, User.Identity.Name, sdkId);
        try
        {
            // Check SDK
            await _sdkForPlayerService.UpdatePlayerSDKStatusCheck(sdk.Id);
            // 检测用户是否拥有该游戏，若拥有Check SDK，但不增加游戏拥有记录。
            if (await _ownerService.GetGameOwnerRecordAsync(userId, sdk.GameItemSDK.GameShopItem.GameInfoId) == null)
                await _ownerService.AddGameOwnerRecordAsync(userId, sdk.GameItemSDK.GameShopItem.GameInfoId);

            var saveResponse = await _unitOfWorkService.SaveEntitiesAsync();
            if (saveResponse == false)
            {
                _logger.LogError("user check game's SDK error, skdId:{SDKId}", sdkId);
                throw new GameRepoDomainException("用户校验游戏SDK失败");
            }
        }
        catch (MySqlException ex)
        {
            _logger.LogError("user:{UserId}-{UserName} check itself sdk:{SDKId} error -> ErrorMessage:{Message}", userId, User.Identity.Name, sdkId, ex.Message);
            throw new GameRepoDomainException("数据库错误，用户校验游戏SDK失败", ex.InnerException);
        }
        _logger.LogInformation("user:{UserId}-{UserName} check itself sdk:{SDKId} successfully", userId, User.Identity.Name, sdkId);
        return NoContent();
    }

    /// <summary>
    /// 用户——获取用户对某个游戏的评分
    /// </summary>
    /// <param name="gameId">游戏Id</param>
    /// <returns></returns>
    [HttpGet("game/user/score/{gameId:int}")]
    [Authorize]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(GameOwner), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetGameScoreAsync([FromRoute] int gameId)
    {
        if (gameId <= 0 || gameId >= int.MaxValue) return BadRequest();
        var userId = User.FindFirstValue("sub");
        var gameRecord = await _ownerService.GetGameOwnerRecordAsync(userId, gameId);
        return gameRecord == null ? NotFound() : Ok(gameRecord);
    }

    /// <summary>
    /// 用户——对游戏进行评分
    /// </summary>
    /// <param name="gameScoreDto"></param>
    /// <returns></returns>
    [HttpPut("game/user/score")]
    [Authorize]
    public async Task<IActionResult> PutGameScoreAsync([FromBody] GameScoreAddDto gameScoreDto)
    {
        var userId = User.FindFirstValue("sub");
        var gameRecord = await _ownerService.GetGameOwnerRecordAsync(userId, gameScoreDto.GameId);
        if (gameRecord == null) return NotFound();

        try
        {
            // 更新用户对该游戏的评分记录
            await _ownerService.UpdateGameScoreAsync(userId, gameScoreDto.GameId, gameScoreDto.GameScore);
            _logger.LogInformation("User rated the game, start to calculate game score");

            // 开始重新计算该游戏的总评游戏分数
            var finalScore = await _ownerService.CalculateGameScore(gameScoreDto.GameId);
            var gameInfo = await _gameInfoService.GetGameInfoAsync(gameScoreDto.GameId);
            gameInfo.AverageScore = finalScore;
            // 更新游戏的总评分
            await _gameInfoService.UpdateGameInfoAsync(gameInfo);

            // 事务提交
            var saveResponse = await _unitOfWorkService.SaveEntitiesAsync();
            if (saveResponse == false)
            {
                _logger.LogError("Game score rate error, database transaction roll back");
                throw new GameRepoDomainException("用户游戏评分失败");
            }
        }
        catch (MySqlException ex)
        {
            _logger.LogError("When game score rate , database occurred a error -> Message:{Message}", ex.Message);
            throw new GameRepoDomainException("用户对游戏评分时，数据库出现错误");
        }

        _logger.LogInformation("user:{UserId} give the game:{GameId} {Score} score", userId, gameScoreDto.GameId, gameScoreDto.GameScore);
        return NoContent();
    }

    /// <summary>
    /// 用户——当购买游戏时发放SDK(供Order服务调用)
    /// </summary>
    /// <param name="addDto"></param>
    /// <returns></returns>
    /// <exception cref="GameRepoDomainException"></exception>
    [HttpPost("user/sdk/send")]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> SendSdkToBuyerAsync([FromBody] SDKPlayerAddDto addDto)
    {
        // 获取该游戏未发放的SDK 准备发送给用户
        _logger.LogInformation("User:{UserID}-{UserName} Buy a ShopItem:{ShopId}, ready to send Sdk", User.FindFirstValue("sub"), User.Identity.Name, addDto.ShopItemId);

        var sdk = await _sdkSendService.GetOneSDKToSendUserAsync(addDto.ShopItemId);
        if (sdk == null) return BadRequest();

        _logger.LogInformation("Get SDK successfully, SDK:{SDKId}-{SDKValue}", sdk.Id, sdk.SDKString);

        // 检查该SDK是否被发放到其他用户
        var ifSend = await _sdkForPlayerService.GetPlayerSDKByIdAsync(sdk.Id);
        if (ifSend != null)
        {
            _logger.LogError("SDK:{SDKId}-{SDKValue} has been owner by another user:{AnotherUserId}, send fail, please fix it", sdk.Id, sdk.SDKString, ifSend.UserId);
            throw new GameRepoDomainException("该SDK已被其他玩家拥有，请联系管理员给你重发");
        }

        // 发放SDK至该用户
        var response = await _sdkForPlayerService.AddPlayerSDKAsync(sdk.Id, addDto.UserId);
        if (response == false)
        {
            _logger.LogError("User:{UserID}, SDK:{SDKId}-{SDKValue} send fail, maybe is Database error", addDto.UserId, sdk.Id, sdk.SDKString);
            throw new GameRepoDomainException("SDK发放失败，请联系管理员给你重发");
        }

        return Ok();
    }
}