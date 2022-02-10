namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class SDKForPlayerService : ISDKForPlayerService
{
    private readonly GameRepoContext _repoDbContext;

    public SDKForPlayerService(GameRepoContext repoDbContext)
    {
        _repoDbContext = repoDbContext ?? throw new ArgumentNullException(nameof(repoDbContext));
    }

    public async Task<List<PlaySDKDto>> GetPlayerSDKByUserIdAsync(string userId, int pageSize, int pageIndex)
    {
        return await _repoDbContext.GameSDKForPlayers
            .Include(sdk => sdk.GameItemSDK)
            .ThenInclude(item => item.GameShopItem)
            .ThenInclude(info => info.GameInfo)
            .Where(x => x.UserId == userId)
            .Select(x => new PlaySDKDto
            {
                SDKString = x.GameItemSDK.SDKString,
                GameItemName = x.GameItemSDK.GameShopItem.GameInfo.Name,
                SendTime = x.GameItemSDK.SendTime.Value,
            })
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(x => x.SendTime)
            .ToListAsync();

    }

    public async Task<List<PlaySDKDto>> GetPlayerSDKByUserIdAndStatusAsync(string userId, int pageSize, int pageIndex, bool? hasChecked)
    {
        var queryString = _repoDbContext.GameSDKForPlayers
            .Include(sdk => sdk.GameItemSDK)
            .ThenInclude(item => item.GameShopItem)
            .ThenInclude(info => info.GameInfo)
            .Where(x => x.UserId == userId);

        _ = hasChecked switch
        {
            true => queryString.Where(x => x.HasChecked == true),
            _ => queryString.Where(x => x.HasChecked == null || x.HasChecked == false)
        };

        return await queryString
            .Select(x => new PlaySDKDto
            {
                SDKString = x.GameItemSDK.SDKString,
                GameItemName = x.GameItemSDK.GameShopItem.GameInfo.Name,
                SendTime = x.GameItemSDK.SendTime.Value
            })
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(x => x.SendTime)
            .ToListAsync();
    }

    public async Task<GameSDKForPlayer> GetPlayerSDKByIdAsync(int id)
    {
        return await _repoDbContext.GameSDKForPlayers
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> UpdatePlayerSDKStatusCheck(int id)
    {
        var recordToUpdate = await _repoDbContext.GameSDKForPlayers.FindAsync(id);

        if (recordToUpdate == null) return false;
        if (recordToUpdate.HasChecked == true) return true;
        
        recordToUpdate.HasChecked = true;
        recordToUpdate.CheckTime = DateTime.Now.ToLocalTime();
        return await _repoDbContext.SaveChangesAsync() > 0;
    }

    public async Task<int> CountPlayerSDKByUserId(string userId)
    {
        return await _repoDbContext.GameSDKForPlayers.CountAsync(x => x.UserId == userId);
    }
}