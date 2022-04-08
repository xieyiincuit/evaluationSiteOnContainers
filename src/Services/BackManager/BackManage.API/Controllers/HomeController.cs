namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index() => new RedirectResult("~/swagger");

    [HttpGet("api/health")]
    public IActionResult Get() => Ok();
}