namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet("api/health")]
    public IActionResult Get() => Ok();
}