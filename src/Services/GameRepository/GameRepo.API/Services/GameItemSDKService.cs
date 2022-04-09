namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class GameItemSDKService : IGameItemSDKService
{
    private readonly ILogger<GameItemSDKService> _logger;
    private readonly GameRepoContext _repoDbContext;

    public GameItemSDKService(GameRepoContext repoDbContext, ILogger<GameItemSDKService> logger)
    {
        _repoDbContext = repoDbContext ?? throw new ArgumentNullException(nameof(repoDbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<GameItemSDK>> GetSDKListByGameItemAsync(int pageIndex, int pageSize, int shopItemId, bool? hasSend)
    {
        var queryString = _repoDbContext.GameItemSDKs
            .AsNoTracking()
            .Where(x => x.GameItemId == shopItemId)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);

        var result = hasSend switch
        {
            true => await queryString.Where(x => x.HasSend == true).ToListAsync(),
            _ => await queryString.Where(x => x.HasSend == null || x.HasSend == false).ToListAsync()
        };
        return result;
    }

    public async Task<long> CountSDKNumberByGameItemOrStatusAsync(int shopItemId, bool? hasSend)
    {
        if (hasSend == null || hasSend == false)
        {
            return await _repoDbContext.GameItemSDKs.LongCountAsync(x => x.GameItemId == shopItemId && (x.HasSend == null || x.HasSend == false));
        }
        return await _repoDbContext.GameItemSDKs.LongCountAsync(x => x.GameItemId == shopItemId && x.HasSend == true);
    }

    public async Task GenerateSDKForGameShopItemAsync(int count, int shopItemId)
    {
        var sdkToInsert = new List<GameItemSDK>();
        for (var i = 0; i < count; i++)
            sdkToInsert.Add(new GameItemSDK
            {
                GameItemId = shopItemId,
                SDKString = shopItemId + Guid.NewGuid().ToString("D")
            });

        await _repoDbContext.GameItemSDKs.AddRangeAsync(sdkToInsert);
    }

    public async Task<int> BatchUpdateSDKStatusAsync(List<int> sdkIds)
    {
        if (sdkIds.Count == 1)
        {
            var sdkItem = await _repoDbContext.GameItemSDKs.Where(x => x.Id == sdkIds[0]).FirstOrDefaultAsync();
            if (sdkItem == null) return 0;
            sdkItem.HasSend = true;
            sdkItem.SendTime = DateTime.Now.ToLocalTime();
        }
        else
        {
            var endRange = sdkIds[^1];
            var sdkItems = await _repoDbContext.GameItemSDKs
                .Where(x => x.Id >= sdkIds[0] && x.Id <= sdkIds[endRange])
                .ToListAsync();
            if (!sdkItems.Any()) return 0;
            foreach (var sdkItem in sdkItems)
            {
                sdkItem.HasSend = true;
                sdkItem.SendTime = DateTime.Now.ToLocalTime();
            }
        }

        return await _repoDbContext.SaveChangesAsync();
    }

    public async Task BatchDeleteGameItemsSDKAsync(int shopItemId, bool? hasSend, int deleteCount)
    {
        var sdkItemsToDelete = await _repoDbContext.GameItemSDKs
            .Where(x => x.GameItemId == shopItemId && x.HasSend == hasSend)
            .OrderBy(x => x.Id)
            .Take(deleteCount)
            .ToListAsync();

        _repoDbContext.GameItemSDKs.RemoveRange(sdkItemsToDelete);
    }

    public async Task<GameItemSDK> GetOneSDKToSendUserAsync(int shopItemId)
    {
        var saved = false;
        var sdk = new GameItemSDK();
        while (!saved)
            try
            {
                sdk = await _repoDbContext.GameItemSDKs.FirstOrDefaultAsync(x => x.HasSend == null && x.GameItemId == shopItemId);
                sdk.HasSend = true;
                sdk.SendTime = DateTime.Now.ToLocalTime();
                await _repoDbContext.SaveChangesAsync();
                saved = true;
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning("Maybe get the same sdk for different user");
            }

        return sdk;
    }
}