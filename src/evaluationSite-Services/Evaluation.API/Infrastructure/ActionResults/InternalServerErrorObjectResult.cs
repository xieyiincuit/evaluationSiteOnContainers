namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.ActionResults;

public class InternalServerErrorObjectResult : ObjectResult
{
    public InternalServerErrorObjectResult(object error) : base(error)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}
