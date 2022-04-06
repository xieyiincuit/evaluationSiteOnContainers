namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class GameShopItemService : IGameShopItemService
{
    private readonly IRedisDatabase _redisDatabase;
    private readonly GameRepoContext _repoDbContext;

    public GameShopItemService(GameRepoContext repoDbContext, IRedisDatabase redisDatabase)
    {
        _repoDbContext = repoDbContext ?? throw new ArgumentNullException(nameof(repoDbContext));
        _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
    }

    public async Task<List<GameShopItem>> GetGameShopItemListAsync(int pageIndex, int pageSize, int orderBy, bool isAdmin)
    {
        var queryString = _repoDbContext.GameShopItems
            .Include(x => x.GameInfo)
                .ThenInclude(g => g.GameCategory)
            .AsNoTracking();

        //非管理员不显示暂停售卖的Item
        if (!isAdmin)
            queryString = _repoDbContext.GameShopItems
                .Include(x => x.GameInfo)
                    .ThenInclude(g => g.GameCategory)
                .Where(x => x.TemporaryStopSell == null || x.TemporaryStopSell == false)
                .AsNoTracking();

        var result = orderBy switch
        {
            1 => await queryString.OrderBy(x => x.Discount)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(),
            2 => await queryString.OrderByDescending(x => x.HotSellPoint)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(),
            _ => await queryString.OrderByDescending(x => x.GameInfo.SellTime)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync()
        };
        return result;
    }

    public async Task<GameShopItem> GetGameShopItemByIdAsync(int shopItemId)
    {
        return await _repoDbContext.GameShopItems
            .Include(x => x.GameInfo)
                .ThenInclude(g => g.GameCategory)
            .Include(x => x.GameInfo)
                .ThenInclude(g => g.GameCompany)
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

    public async Task<bool> AddGameShopItemAsync(GameShopItem gameShopItem)
    {
        await _repoDbContext.GameShopItems.AddAsync(gameShopItem);
        return await _repoDbContext.SaveChangesAsync() > 0;
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
        {
            shopItemForUpdate.TemporaryStopSell = true;
            await _redisDatabase.RemoveAsync(GetProductStockKey(shopItemId));
        }
        else
        {
            //恢复售卖状态
            shopItemForUpdate.TemporaryStopSell = null;
            await _redisDatabase.Database.StringSetAsync(GetProductStockKey(shopItemId),
                shopItemForUpdate.AvailableStock);
        }

        return await _repoDbContext.SaveChangesAsync() > 0;
    }

    public async Task<int> CountGameShopItemAsync()
    {
        return await _repoDbContext.GameShopItems.CountAsync();
    }

    public async Task<bool> UpdateShopItemStockWhenTakeDownAsync(int shopItemId)
    {
        if (await _redisDatabase.ExistsAsync(GetProductStockKey(shopItemId)) == false) return false;
        var itemStock = await _redisDatabase.GetAsync<int>(GetProductStockKey(shopItemId));

        var shopItem = await _repoDbContext.GameShopItems.FindAsync(shopItemId);
        if (shopItem == null) return false;
        if (shopItem.AvailableStock <= itemStock) return false;
        shopItem.AvailableStock = itemStock;
        return await _repoDbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateShopItemStockWhenChangeNumberAsync(int shopItemId, int newStock)
    {
        var shopItem = await _repoDbContext.GameShopItems.FindAsync(shopItemId);
        if (shopItem == null) return false;
        shopItem.AvailableStock = newStock;
        return await _repoDbContext.SaveChangesAsync() > 0;
    }

    private string GetProductStockKey(int productId)
    {
        return $"ProductStock_{productId}";
    }
}