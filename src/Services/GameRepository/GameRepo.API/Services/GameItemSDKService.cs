namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class GameItemSDKService : IGameItemSDKService
{
    private readonly GameRepoContext _repoDbContext;

    public GameItemSDKService(GameRepoContext repoDbContext)
    {
        _repoDbContext = repoDbContext ?? throw new ArgumentNullException(nameof(repoDbContext));
    }

    public async Task<List<GameItemSDK>> GetSDKListByGameItemAsync(int pageIndex, int pageSize, int gameItemId, bool? hasSend)
    {
        var queryString = _repoDbContext.GameItemSDKs
            .AsNoTracking()
            .Where(x => x.GameItemId == gameItemId)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);

        var result = hasSend switch
        {
            true => await queryString.Where(x => x.HasSend == true).ToListAsync(),
            _ => await queryString.Where(x => x.HasSend == null || x.HasSend == false).ToListAsync()
        };
        return result;
    }

    public async Task<long> CountSDKNumberByGameItemOrStatusAsync(int gameItemId, bool? hasSend)
    {
        if (hasSend == null || hasSend == false)
            return await _repoDbContext.GameItemSDKs
                .LongCountAsync(x =>
                    x.GameItemId == gameItemId && (x.HasSend == null || x.HasSend == false));
        else
            return await _repoDbContext.GameItemSDKs
                .LongCountAsync(x => x.GameItemId == gameItemId && (x.HasSend == true));
    }

    public async Task<int> GenerateSDKForGameShopItemAsync(int count, int gameItemId)
    {
        var sdkToInsert = new List<GameItemSDK>();
        for (var i = 0; i < count; i++)
        {
            sdkToInsert.Add(new GameItemSDK
            {
                GameItemId = gameItemId,
                SDKString = gameItemId + Guid.NewGuid().ToString("D")
            });
        }

        await _repoDbContext.GameItemSDKs.AddRangeAsync(sdkToInsert);
        return await _repoDbContext.SaveChangesAsync();
    }

    public async Task<int> BatchUpdateSDKStatusAsync(List<int> sdkIds)
    {
        if (sdkIds.Count == 1)
        {
            var sdkItem = await _repoDbContext.GameItemSDKs.Where(x => x.Id == sdkIds[0]).FirstOrDefaultAsync();
            sdkItem.HasSend = true;
            sdkItem.SendTime = DateTime.Now.ToLocalTime();
        }
        else
        {
            var endRange = sdkIds[^1];
            var sdkItems = await _repoDbContext.GameItemSDKs
                .Where(x => x.Id >= sdkIds[0] && x.Id <= sdkIds[endRange])
                .ToListAsync();
            foreach (var sdkItem in sdkItems)
            {
                sdkItem.HasSend = true;
                sdkItem.SendTime = DateTime.Now.ToLocalTime();
            }
        }

        return await _repoDbContext.SaveChangesAsync();
    }

    public async Task<int> BatchDeleteGameItemsSDKAsync(int gameItemId, bool? hasSend)
    {
        var sdkItemsToDelete = await _repoDbContext.GameItemSDKs
            .Where(x => x.GameItemId == gameItemId && x.HasSend == hasSend)
            .ToListAsync();

        _repoDbContext.GameItemSDKs.RemoveRange(sdkItemsToDelete);
        return await _repoDbContext.SaveChangesAsync();
    }
}