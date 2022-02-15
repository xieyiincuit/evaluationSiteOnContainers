namespace BackManage.API.Controllers;

[ApiController]
[Route("api/v1/b")]
public class ApproveController : ControllerBase
{
    private readonly ILogger<ApproveController> _logger;

    public ApproveController(ILogger<ApproveController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    [HttpPost("approve/{userId:alpha}")]
    public async Task<IActionResult> ApprovedUserToEvaluatorAsync([FromRoute] string userId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("band/{userId:alpha}")]
    public async Task<IActionResult> BandUserAsync([FromRoute] string userId)
    {
        throw new NotImplementedException();
    }
}
