namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

public class HomeController : ControllerBase
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    public IActionResult Index() => new RedirectResult("~/swagger");

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("api/health")]
    public IActionResult Get() => Ok();
}