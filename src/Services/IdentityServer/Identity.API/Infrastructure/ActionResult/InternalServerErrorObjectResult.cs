namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Infrastructure;

public class InternalServerErrorObjectResult : ObjectResult
{
    public InternalServerErrorObjectResult(object error) : base(error)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}
