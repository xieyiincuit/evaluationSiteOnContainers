namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.ActionResults;

public class InternalServerErrorObjectResult : ObjectResult
{
    //用于Fileter全局异常捕获
    public InternalServerErrorObjectResult(object error) : base(error)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}
