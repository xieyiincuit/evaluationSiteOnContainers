namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet("api/health")]
    public IActionResult Get() => Ok();
}