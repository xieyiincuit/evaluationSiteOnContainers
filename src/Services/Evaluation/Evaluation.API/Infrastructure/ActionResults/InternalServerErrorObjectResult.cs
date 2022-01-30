namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.ActionResults;

public class InternalServerErrorObjectResult : ObjectResult
{
    //用于Filter全局异常捕获
    public InternalServerErrorObjectResult(object error) : base(error)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}