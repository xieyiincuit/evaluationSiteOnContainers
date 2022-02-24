namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Infrastructure;

//该服务特定的错误
public class IdentityDomainException : Exception
{
    public IdentityDomainException()
    {
    }

    public IdentityDomainException(string message) : base(message)
    {
    }

    public IdentityDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}