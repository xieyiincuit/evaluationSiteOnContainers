namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Controllers;

/// <summary>
/// 网站统计功能接口
/// </summary>
[ApiController]
[Route("api/v1/back/statistics")]
[Authorize(Roles = "administrator")]
public class StatisticsController : ControllerBase
{
    private readonly EvaluationRepoGrpcService _evalGrpcService;
    private readonly GameRepoGrpcService _gameGrpcService;
    private readonly IdentityHttpClient _identityHttpClient;

    public StatisticsController(
        EvaluationRepoGrpcService evalGrpcService,
        GameRepoGrpcService gameGrpcService,
        IdentityHttpClient identityHttpClient)
    {
        _evalGrpcService = evalGrpcService ?? throw new ArgumentNullException(nameof(evalGrpcService));
        _gameGrpcService = gameGrpcService ?? throw new ArgumentNullException(nameof(gameGrpcService));
        _identityHttpClient = identityHttpClient ?? throw new ArgumentNullException(nameof(identityHttpClient));
    }

    /// <summary>
    /// 管理员——统计测评文章
    /// </summary>
    /// <returns></returns>
    [HttpGet("articles")]
    public async Task<IActionResult> GetArticleInfo()
    {
        var response = await _evalGrpcService.GetArticleStatisticsAsync();
        return Ok(response);
    }

    /// <summary>
    /// 管理员——统计游戏信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("games")]
    public async Task<IActionResult> GetGamesInfo()
    {
        var response = await _gameGrpcService.GetGameStatisticsAsync();
        return Ok(response);
    }

    /// <summary>
    /// 管理员——统计用户信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("users")]
    public async Task<IActionResult> GetUserInfo()
    {
        using var response = await _identityHttpClient.CountUserAsync();
        if (response.IsSuccessStatusCode)
        {
            var userCountDto = await response.Content.ReadFromJsonAsync<UserCountDto>();
            return Ok(userCountDto);
        }
        return BadRequest();
    }
}