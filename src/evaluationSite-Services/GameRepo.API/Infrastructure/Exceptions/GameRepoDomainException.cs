namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Infrastructure.Exceptions;

//该服务特定的错误
public class GameRepoDomainException : Exception
{
    public GameRepoDomainException() { }

    public GameRepoDomainException(string message) : base(message) { }

    public GameRepoDomainException(string message, Exception innerException) : base(message, innerException) { }
}