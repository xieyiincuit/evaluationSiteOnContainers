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

    public async Task<List<GameRankDto>> GetGameInfoRankAsync()
    {
        return await _repoContext.GameInfos
            .Select(x => new GameRankDto() { GameId = x.Id, GameName = x.Name, GameScore = x.AverageScore })
            .OrderByDescending(x => x.GameScore)
            .Take(10)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<GameSelectDto>> GetGameSelectAsync()
    {
        return await _repoContext.GameInfos
            .Select(x => new GameSelectDto() { GameId = x.Id, GameName = x.Name })
            .AsNoTracking()
            .ToListAsync();
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
            .OrderBy(x => x.Name)
            .AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<GameInfo>> GetGameInfoWithTermAsync(int pageIndex, int pageSize, int? categoryId, int? companyId, string order = "hot")
    {
        var queryString = _repoContext.GameInfos
            .AsNoTracking();

        //Both
        if (categoryId.HasValue && categoryId.Value != 0 && companyId.HasValue && companyId.Value != 0)
        {
            queryString = queryString.Where(x => x.GameCategoryId == categoryId && x.GameCompanyId == companyId);
        }
        else if (categoryId.HasValue && categoryId.Value != 0 || companyId.HasValue && companyId.Value != 0) // One of them
        {
            if (companyId.HasValue && companyId.Value != 0)
                queryString = queryString.Where(x => x.GameCompanyId == companyId);
            else
                queryString = queryString.Where(x => x.GameCategoryId == categoryId);
        }

        queryString = order switch
        {
            "hot" => queryString.OrderByDescending(x => x.HotPoints),
            "time" => queryString.OrderByDescending(x => x.SellTime),
            "score" => queryString.OrderByDescending(x => x.AverageScore),
            _ => queryString.OrderBy(x => x.Name),
        };

        return await queryString
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