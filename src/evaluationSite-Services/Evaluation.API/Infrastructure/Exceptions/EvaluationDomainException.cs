namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.Exceptions;

//该服务特定的错误
public class EvaluationDomainException : Exception
{
    public EvaluationDomainException()
    {
    }

    public EvaluationDomainException(string message) : base(message)
    {
    }

    public EvaluationDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}