namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet("api/health")]
    public IActionResult Get() => Ok();
}