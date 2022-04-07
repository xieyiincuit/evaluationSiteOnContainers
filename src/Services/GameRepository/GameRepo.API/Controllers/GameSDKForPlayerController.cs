namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1")]
public class GameSDKForPlayerController : ControllerBase
{
    private const int _pageSize = 10;
    private readonly ILogger<GameSDKForPlayerController> _logger;
    private readonly IGameOwnerService _ownerService;
    private readonly ISDKForPlayerService _sdkForPlayerService;
    private readonly IGameItemSDKService _sdkSendService;
    private readonly IUnitOfWorkService _unitOfWorkService;
    private readonly IGameInfoService _gameInfoService;

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

    [HttpGet("game/user/sdks")]
    [Authorize]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<PlaySDKDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserSDKsByUserIdAsync([FromQuery] bool? hasChecked, [FromQuery] int pageIndex = 1)
    {
        var userId = User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId)) return BadRequest();

        var userSdkCount = await _sdkForPlayerService.CountPlayerSDKByUserId(userId);
        if (userSdkCount == 0) return NotFound();
        if (!ParameterValidateHelper.IsInvalidPageIndex(userSdkCount, pageIndex, _pageSize)) pageIndex = 1;

        var userSdksDto = hasChecked switch
        {
            false => await _sdkForPlayerService.GetPlayerSDKByUserIdAndStatusAsync(userId, _pageSize, pageIndex, null),
            true => await _sdkForPlayerService.GetPlayerSDKByUserIdAndStatusAsync(userId, _pageSize, pageIndex, true),
            _ => await _sdkForPlayerService.GetPlayerSDKByUserIdAsync(userId, _pageSize, pageIndex)
        };
        _logger.LogInformation("user:{UserId}, get it sdkList", userId);
        var paginatedModel = new PaginatedItemsDtoModel<PlaySDKDto>(pageIndex, _pageSize, userSdkCount, userSdksDto);
        return Ok(paginatedModel);
    }

    [HttpGet("game/sdk/{sdkId:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(GameItemSDK), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSDKByIdAsync([FromRoute] int sdkId)
    {
        if (sdkId <= 0 || sdkId >= int.MaxValue) return BadRequest();

        var sdk = await _sdkForPlayerService.GetPlayerSDKByIdAsync(sdkId);
        return sdk == null ? NotFound() : Ok(sdk);
    }

    [HttpPut("game/user/sdk/{sdkId:int}")]
    [Authorize]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CheckUserSdkAsync([FromRoute] int sdkId)
    {
        if (sdkId <= 0 || sdkId >= int.MaxValue) return BadRequest();

        var sdk = await _sdkForPlayerService.GetPlayerSDKByIdAsync(sdkId);
        var userId = User.FindFirstValue("sub");
        if (sdk.UserId != userId) return BadRequest();

        // Check SDK
        await _sdkForPlayerService.UpdatePlayerSDKStatusCheck(sdk.Id);

        // 检测用户是否拥有该游戏，若拥有只Check SDK，而不增加游戏拥有记录。
        if (await _ownerService.GetGameOwnerRecordAsync(userId, sdk.GameItemSDK.GameShopItem.GameInfo.Id) == null)
        {
            await _ownerService.AddGameOwnerRecordAsync(userId, sdk.GameItemSDK.GameShopItem.GameInfoId);
        }

        var response = await _unitOfWorkService.SaveEntitiesAsync();
        return response == true ? NoContent() : BadRequest();
    }


    [HttpGet("game/user/score/{gameId:int}")]
    [Authorize]
    public async Task<IActionResult> GetGameScoreAsync([FromRoute] int gameId)
    {
        if (gameId <= 0 || gameId >= int.MaxValue) return BadRequest();
        var userId = User.FindFirstValue("sub");

        var gameRecord = await _ownerService.GetGameOwnerRecordAsync(userId, gameId);
        if (gameRecord == null) return NotFound();

        return Ok(gameRecord);
    }

    [HttpPut("game/user/score")]
    [Authorize]
    public async Task<IActionResult> PutGameScoreAsync([FromBody] GameScoreAddDto gameScoreDto)
    {
        var userId = User.FindFirstValue("sub");
        var gameRecord = await _ownerService.GetGameOwnerRecordAsync(userId, gameScoreDto.GameId);
        if (gameRecord == null) return NotFound();

        await _ownerService.UpdateGameScoreAsync(userId, gameScoreDto.GameId, gameScoreDto.GameScore);
        
        //更新游戏分数
        var finalScore = await _ownerService.CalculateGameScore(gameScoreDto.GameId);
        var gameInfo = await _gameInfoService.GetGameInfoAsync(gameScoreDto.GameId);
        gameInfo.AverageScore = finalScore;
        await _gameInfoService.UpdateGameInfoAsync(gameInfo);
        await _unitOfWorkService.SaveChangesAsync();

        _logger.LogInformation("user:{UserId} give the game:{GameId} {Score} score", userId, gameScoreDto.GameId, gameScoreDto.GameScore);
        return NoContent();
    }

    [HttpPost("user/sdk/send")]
    [Authorize]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SendSdkToBuyerAsync([FromBody] SDKPlayerAddDto addDto)
    {
        var sdk = await _sdkSendService.GetOneSDKToSendUserAsync(addDto.ShopItemId);
        if (sdk == null) return BadRequest();

        var ifSend = await _sdkForPlayerService.GetPlayerSDKByIdAsync(sdk.Id);
        if (ifSend != null) throw new GameRepoDomainException("该SDK已被使用，请联系管理员给你重发");

        var response = await _sdkForPlayerService.AddPlayerSDKAsync(sdk.Id, addDto.UserId);
        if (response == false) throw new GameRepoDomainException("SDK发放失败，请联系管理员给你重发");

        //这时候SDK的状态已经成为Send 并且SDKPlayer表中也已经有记录了, 则已经购买成功。

        return Ok();
    }
}