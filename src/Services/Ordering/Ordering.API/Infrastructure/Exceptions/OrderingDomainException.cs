namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.Infrastructure;

//该服务特定的错误
public class OrderingDomainException : Exception
{
    public OrderingDomainException() { }

    public OrderingDomainException(string message) : base(message) { }

    public OrderingDomainException(string message, Exception innerException) : base(message, innerException) { }
}