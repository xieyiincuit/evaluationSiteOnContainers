namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IUnitOfWorkService
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}