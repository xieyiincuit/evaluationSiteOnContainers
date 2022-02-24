namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/game/shop/sdks")]
public class GameSDKController : ControllerBase
{
    private const int _pageSize = 15;
    private readonly ILogger<GameSDKController> _logger;
    private readonly IGameItemSDKService _sdkService;

    public GameSDKController(IGameItemSDKService sdkService, ILogger<GameSDKController> logger)
    {
        _sdkService = sdkService ?? throw new ArgumentNullException(nameof(sdkService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{gameItemId:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<GameItemSDK>), (int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetGameSDKAsync(
        [FromQuery] bool hasSend, [FromRoute] int gameItemId, [FromQuery] int pageIndex = 1)
    {
        var sdkCount = await _sdkService.CountSDKNumberByGameItemOrStatusAsync(gameItemId, hasSend);
        if (ParameterValidateHelper.IsInvalidPageIndex(sdkCount, _pageSize, pageIndex)) pageIndex = 1;

        var sdks = await _sdkService.GetSDKListByGameItemAsync(pageIndex, _pageSize, gameItemId, hasSend);
        if (!sdks.Any()) return NotFound();

        var model = new PaginatedItemsDtoModel<GameItemSDK>(pageIndex, _pageSize, sdkCount, sdks);
        return Ok(model);
    }

    [HttpPut]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> UpdateSDKStatusAsync(List<int> sdkIds)
    {
        if (sdkIds == null) return BadRequest();

        var response = await _sdkService.BatchUpdateSDKStatusAsync(sdkIds);
        return response > 1 ? NoContent() : BadRequest();
    }
}