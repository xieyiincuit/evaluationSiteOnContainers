namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Controllers;

public class HomeController : Controller
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    public IActionResult Index() => new RedirectResult("~/swagger");

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("api/health")]
    public IActionResult Get() => Ok();
}