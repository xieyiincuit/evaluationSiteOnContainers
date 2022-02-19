namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Infrastructure;

//该服务特定的错误
public class BackManageDomainException : Exception
{
    public BackManageDomainException() { }

    public BackManageDomainException(string message) : base(message) { }

    public BackManageDomainException(string message, Exception innerException) : base(message, innerException) { }
}