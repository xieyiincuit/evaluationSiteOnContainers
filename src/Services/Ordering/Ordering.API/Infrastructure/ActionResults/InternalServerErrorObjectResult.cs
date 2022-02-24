namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.Infrastructure;

public class InternalServerErrorObjectResult : ObjectResult
{
    public InternalServerErrorObjectResult(object error) : base(error)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}