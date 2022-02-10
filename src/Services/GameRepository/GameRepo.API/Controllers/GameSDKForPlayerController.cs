namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1")]
public class GameSDKForPlayerController : ControllerBase
{
    private readonly ISDKForPlayerService _sdkForPlayerService;
    private readonly ILogger<GameSDKForPlayerController> _logger;
    private readonly IGameItemSDKService _sdkSendService;
    private readonly IGameOwnerService _ownerService;
    private readonly IUnitOfWorkService _unitOfWorkService;
    private const int _pageSize = 10;

    public GameSDKForPlayerController(
        ISDKForPlayerService sdkForPlayerService,
        ILogger<GameSDKForPlayerController> logger,
        IGameItemSDKService sdkSendService,
        IGameOwnerService ownerService,
        IUnitOfWorkService unitOfWorkService)
    {
        _sdkForPlayerService = sdkForPlayerService ?? throw new ArgumentNullException(nameof(sdkForPlayerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _sdkSendService = sdkSendService ?? throw new ArgumentNullException(nameof(sdkSendService));
        _ownerService = ownerService ?? throw new ArgumentNullException(nameof(ownerService));
        _unitOfWorkService = unitOfWorkService ?? throw new ArgumentNullException(nameof(unitOfWorkService));
    }

    [HttpGet("u/sdks")]
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
            false => await _sdkForPlayerService.GetPlayerSDKByUserIdAndStatusAsync(userId, _pageSize, pageIndex, false),
            true => await _sdkForPlayerService.GetPlayerSDKByUserIdAndStatusAsync(userId, _pageSize, pageIndex, true),
            _ => await _sdkForPlayerService.GetPlayerSDKByUserIdAsync(userId, _pageSize, pageIndex)
        };

        var paginatedModel = new PaginatedItemsDtoModel<PlaySDKDto>(pageIndex, _pageSize, userSdkCount, userSdksDto);
        return Ok(paginatedModel);
    }

    [HttpGet("g/sdk/{sdkId:int}")]
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

    [HttpPut("u/sdk/{sdkId:int}")]
    [Authorize]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CheckUserSdkAsync([FromRoute] int sdkId)
    {
        if (sdkId <= 0 || sdkId >= int.MaxValue) return BadRequest();

        var sdk = await _sdkForPlayerService.GetPlayerSDKByIdAsync(sdkId);
        var userId = User.FindFirstValue("sub");
        if (sdk.UserId != userId) return BadRequest();

        await _sdkForPlayerService.UpdatePlayerSDKStatusCheck(sdkId);
        await _ownerService.AddGameOwnerRecordAsync(userId, sdk.GameItemSDK.GameShopItem.Id);
        var response = await _unitOfWorkService.SaveEntitiesAsync();
        return response == true ? NoContent() : BadRequest();
    }

    [HttpPost("u/sdk/send")]
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