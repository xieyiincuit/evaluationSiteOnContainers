namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

/// <summary>
/// 游戏SDK管理接口
/// </summary>
[ApiController]
[Route("api/v1/game")]
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

    /// <summary>
    /// 管理员——分页获取游戏SDK信息
    /// </summary>
    /// <param name="hasSend">是否已经发出</param>
    /// <param name="gameItemId">属于哪个游戏</param>
    /// <param name="pageIndex">pageSize=15</param>
    /// <returns></returns>
    [HttpGet("shop/sdks/{gameItemId:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<GameItemSDK>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetGameSDKAsync([FromQuery] bool hasSend, [FromRoute] int gameItemId, [FromQuery] int pageIndex = 1)
    {
        var sdkCount = await _sdkService.CountSDKNumberByGameItemOrStatusAsync(gameItemId, hasSend);
        if (ParameterValidateHelper.IsInvalidPageIndex(sdkCount, _pageSize, pageIndex)) pageIndex = 1;

        var sdks = await _sdkService.GetSDKListByGameItemAsync(pageIndex, _pageSize, gameItemId, hasSend);
        if (!sdks.Any()) return NotFound();

        var model = new PaginatedItemsDtoModel<GameItemSDK>(pageIndex, _pageSize, sdkCount, sdks);
        return Ok(model);
    }

    /// <summary>
    /// 管理员——批量校验SDK(业务中暂时没有用到此方法)
    /// </summary>
    /// <param name="sdkIds">sdkId的数组</param>
    /// <returns></returns>
    [HttpPut("shop/sdks")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> UpdateSDKStatusAsync(List<int> sdkIds)
    {
        if (sdkIds == null) return BadRequest();

        var updateResponse = await _sdkService.BatchUpdateSDKStatusAsync(sdkIds);
        if (updateResponse < 1)
        {
            _logger.LogError("administrator: id:{id}, name:{Name} batchUpdate sdks error -> sdks:{@sdks}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), sdkIds);
            throw new GameRepoDomainException("批量改变SDK状态失败");
        }

        return NoContent();
    }
}