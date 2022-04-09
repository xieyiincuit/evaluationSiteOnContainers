namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class GameShopItemService : IGameShopItemService
{
    private readonly GameRepoContext _repoDbContext;

    public GameShopItemService(GameRepoContext repoDbContext)
    {
        _repoDbContext = repoDbContext ?? throw new ArgumentNullException(nameof(repoDbContext));
    }

    public async Task<int> CountGameShopItemAsync()
    {
        return await _repoDbContext.GameShopItems.CountAsync();
    }

    public async Task<ShopItemStatusDto> CheckShopStatusAsync(int shopItemId)
    {
        return await _repoDbContext.GameShopItems
            .AsNoTracking()
            .Where(x=>x.Id == shopItemId)
            .Select(x => new ShopItemStatusDto() { Id = x.Id, TemporaryStopSell = x.TemporaryStopSell })
            .FirstOrDefaultAsync();
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

    public async Task AddGameShopItemAsync(GameShopItem gameShopItem)
    {
        await _repoDbContext.GameShopItems.AddAsync(gameShopItem);
    }

    public async Task DeleteGameShopItemByIdAsync(int shopItemId)
    {
        var shopItemForDelete = await _repoDbContext.GameShopItems.FindAsync(shopItemId);
        _repoDbContext.GameShopItems.Remove(shopItemForDelete);
    }

    public async Task<bool> UpdateGameShopItemInfoAsync(GameShopItem gameShopItem)
    {
        _repoDbContext.GameShopItems.Update(gameShopItem);
        return await _repoDbContext.SaveChangesAsync() > 0;
    }

    public async Task TakeDownGameShopItemAsync(int shopItemId)
    {
        var shopItemForUpdate = await _repoDbContext.GameShopItems.FindAsync(shopItemId);
        shopItemForUpdate.TemporaryStopSell = true; //暂停销售
    }

    public async Task<int> TakeUpGameShopItemAsync(int shopItemId)
    {
        var shopItemForUpdate = await _repoDbContext.GameShopItems.FindAsync(shopItemId);
        shopItemForUpdate.TemporaryStopSell = false; //开始销售
        return shopItemForUpdate.AvailableStock;
    }

    public async Task UpdateShopItemStockWhenTakeDownAsync(int shopItemId, int currentStock)
    {
        var shopItem = await _repoDbContext.GameShopItems.FindAsync(shopItemId);
        shopItem.AvailableStock = currentStock;
    }

    public async Task UpdateShopItemStockWhenChangeNumberAsync(int shopItemId, int newStock)
    {
        var shopItem = await _repoDbContext.GameShopItems.FindAsync(shopItemId);
        shopItem.AvailableStock = newStock;
    }
}