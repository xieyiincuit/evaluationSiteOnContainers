namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet("api/health")]
    public IActionResult Get() => Ok();
}