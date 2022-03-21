namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class GameInfoService : IGameInfoService
{
    private readonly GameRepoContext _repoContext;

    public GameInfoService(GameRepoContext repoContext)
    {
        _repoContext = repoContext ?? throw new ArgumentNullException(nameof(repoContext));
    }

    public async Task AddGameInfoAsync(GameInfo gameInfo)
    {
        await _repoContext.AddAsync(gameInfo);
    }

    public async Task<int> CountGameInfoAsync()
    {
        return await _repoContext.GameInfos.CountAsync();
    }

    public async Task<GameInfo> GetGameInfoAsync(int gameId)
    {
        return await _repoContext.GameInfos
            .Include(info => info.GameCategory)
            .Include(info => info.GameCompany)
            .Where(x => x.Id == gameId)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<List<GameInfo>> GetGameInfosAsync(int pageIndex, int pageSize)
    {
        return await _repoContext.GameInfos
            .Include(info => info.GameCategory)
            .Include(info => info.GameCompany)
            .OrderBy(x => x.Name)
            .AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task RemoveGameInfoAsync(int gameId)
    {
        var entity = await _repoContext.GameInfos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == gameId);
        if (entity == null)
            return;
        _repoContext.GameInfos.Remove(entity);
    }

    public Task UpdateGameInfoAsync(GameInfo gameInfo)
    {
        _repoContext.GameInfos.Update(gameInfo);
        return Task.CompletedTask;
    }

    public async Task<bool> GameExistAsync(int gameId)
    {
        var game = await _repoContext.GameInfos
            .Where(x => x.Id == gameId)
            .AsNoTracking()
            .FirstOrDefaultAsync();
        return game != null;
    }
}