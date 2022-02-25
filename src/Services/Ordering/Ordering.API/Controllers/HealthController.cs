namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet("api/health")]
    public IActionResult Get() => Ok();
}