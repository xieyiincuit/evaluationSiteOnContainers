namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class GameShopItemService : IGameShopItemService
{
    private readonly GameRepoContext _repoDbContext;

    public GameShopItemService(GameRepoContext repoDbContext)
    {
        _repoDbContext = repoDbContext ?? throw new ArgumentNullException(nameof(repoDbContext));
    }

    public async Task<List<GameShopItem>> GetGameShopItemListAsync(int pageIndex, int pageSize, int orderBy)
    {
        var queryString = _repoDbContext.GameShopItems
            .Include(x => x.GameInfo)
            .AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);

        var result = orderBy switch
        {
            1 => await queryString.OrderBy(x => x.Discount).ToListAsync(),
            2 => await queryString.OrderBy(x => x.HotSellPoint).ToListAsync(),
            _ => await queryString.OrderBy(x => x.Price).ToListAsync()
        };
        return result;
    }

    public async Task<GameShopItem> GetGameShopItemByIdAsync(int shopItemId)
    {
        return await _repoDbContext.GameShopItems
            .Include(x => x.GameInfo)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == shopItemId);
    }

    public async Task<GameShopItem> GetGameShopItemByGameIdAsync(int gameInfoId)
    {
        return await _repoDbContext.GameShopItems
            .Include(x => x.GameInfo)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GameInfoId == gameInfoId);
    }

    public async Task AddGameShopItemAsync(GameShopItem gameShopItem)
    {
        await _repoDbContext.GameShopItems.AddAsync(gameShopItem);
    }

    public async Task<bool> DeleteGameShopItemByIdAsync(int shopItemId)
    {
        var shopItemForDelete = await _repoDbContext.GameShopItems.FindAsync(shopItemId);
        if (shopItemForDelete == null) return false;
        _repoDbContext.GameShopItems.Remove(shopItemForDelete);
        return await _repoDbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateGameShopItemInfoAsync(GameShopItem gameShopItem)
    {
        _repoDbContext.GameShopItems.Update(gameShopItem);
        return await _repoDbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> ChangeGameShopItemStatusAsync(int shopItemId)
    {
        var shopItemForUpdate = await _repoDbContext.GameShopItems.FindAsync(shopItemId);
        if (shopItemForUpdate == null) return false;

        if (shopItemForUpdate.TemporaryStopSell == null || shopItemForUpdate.TemporaryStopSell == false)
            shopItemForUpdate.TemporaryStopSell = true;
        else
            shopItemForUpdate.TemporaryStopSell = false;

        return await _repoDbContext.SaveChangesAsync() > 0;
    }

    public async Task<int> CountGameShopItemAsync()
    {
        return await _repoDbContext.GameShopItems.CountAsync();
    }
}