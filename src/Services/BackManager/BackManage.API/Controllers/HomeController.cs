namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return new RedirectResult("~/swagger");
    }
}
