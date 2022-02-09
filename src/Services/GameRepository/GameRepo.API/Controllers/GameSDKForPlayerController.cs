namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1")]
public class GameSDKForPlayerController : ControllerBase
{
    private readonly ISDKForPlayerService _sdkForPlayerService;
    private readonly ILogger<GameSDKForPlayerController> _logger;
    private const int _pageSize = 10;

    public GameSDKForPlayerController(ISDKForPlayerService sdkForPlayerService, ILogger<GameSDKForPlayerController> logger)
    {
        _sdkForPlayerService = sdkForPlayerService ?? throw new ArgumentNullException(nameof(sdkForPlayerService));
        _logger = logger;
    }

    [HttpGet("u/sdks")]
    [Authorize]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<PlaySDKDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserSDKsByUserIdAsync([FromQuery] int pageIndex, [FromQuery] bool? hasChecked)
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
        if (sdk.UserId != User.FindFirstValue("sub")) return BadRequest();

        var response = await _sdkForPlayerService.UpdatePlayerSDKStatusCheck(sdkId);
        return response == true ? NoContent() : BadRequest();
    }
}