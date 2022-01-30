namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class UnitOfWorkService : IUnitOfWorkService
{
    private readonly GameRepoContext _repoContext;

    public UnitOfWorkService(GameRepoContext repoContext)
    {
        _repoContext = repoContext ?? throw new ArgumentNullException(nameof(repoContext));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _repoContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _repoContext.SaveChangesAsync(cancellationToken) > 0;
    }
}