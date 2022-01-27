namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Extensions.Exceptions;

public class InternalServerErrorObjectResult : ObjectResult
{
    public InternalServerErrorObjectResult(object error) : base(error)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}
