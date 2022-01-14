namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Controllers;

public class HomeController : Controller
{
    // GET: /<controller>/
    public IActionResult Index()
    {
        return new RedirectResult("~/swagger");
    }
}