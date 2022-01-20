namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Services;

public class GameInfoService : IGameInfoService
{
    private readonly GameRepoContext _repoContext;

    public GameInfoService(GameRepoContext repoContext)
    {
        _repoContext = repoContext;
    }

    public async Task<bool> AddGameInfoAsync(GameInfo gameInfo)
    {
        await _repoContext.AddAsync(gameInfo);
        return await _repoContext.SaveChangesAsync() > 0;
    }

    public async Task<int> CountGameInfoAsync()
    {
        return await _repoContext.GameInfos.CountAsync();
    }

    public async Task<GameInfo> GetGameInfoAsync(int gameId)
    {
        return await _repoContext.GameInfos.FindAsync(gameId);
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

    public async Task<bool> RemoveGameInfoAsync(int gameId)
    {
        var entity = await _repoContext.GameInfos.FindAsync(gameId);
        _repoContext.GameInfos.Remove(entity);
        return await _repoContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateGameInfoAsync(GameInfo gameInfo)
    {
        _repoContext.GameInfos.Update(gameInfo);
        return await _repoContext.SaveChangesAsync() > 0;
    }
}
