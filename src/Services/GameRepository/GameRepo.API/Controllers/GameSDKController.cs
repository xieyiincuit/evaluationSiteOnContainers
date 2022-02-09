namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/g")]
public class GameSDKController : ControllerBase
{
    private readonly IGameItemSDKService _sdkService;
    private readonly ILogger<GameSDKController> _logger;
    private const int _pageSize = 15;

    public GameSDKController(IGameItemSDKService sdkService, ILogger<GameSDKController> logger)
    {
        _sdkService = sdkService ?? throw new ArgumentNullException(nameof(sdkService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Route("shop/sdks/{gameItemId:int}")]
    [Authorize("administrator")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<GameItemSDK>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetGameSDKAsync(
        [FromQuery] int pageIndex, [FromQuery] bool hasSend, [FromRoute] int gameItemId)
    {
        var sdkCount = await _sdkService.CountSDKNumberByGameItemOrStatusAsync(gameItemId, hasSend);
        if (ParameterValidateHelper.IsInvalidPageIndex(sdkCount, _pageSize, pageIndex)) pageIndex = 1;

        var sdks = await _sdkService.GetSDKListByGameItemAsync(pageIndex, _pageSize, gameItemId, hasSend);
        if (!sdks.Any()) return NotFound();

        var model = new PaginatedItemsDtoModel<GameItemSDK>(pageIndex, _pageSize, sdkCount, sdks);
        return Ok(model);
    }


    //TODO 考虑用Grpc 考虑这里的权限控制
    [HttpPut]
    [Route("shop/sdks")]
    [Authorize("administrator")]
    public async Task<IActionResult> UpdateSDKStatusAsync(List<int> sdkIds)
    {
        if (sdkIds == null) return BadRequest();
        sdkIds = (List<int>)sdkIds.Distinct();

        var response = await _sdkService.BatchUpdateSDKStatusAsync(sdkIds);
        return response > 1 ? NoContent() : BadRequest();
    }
}