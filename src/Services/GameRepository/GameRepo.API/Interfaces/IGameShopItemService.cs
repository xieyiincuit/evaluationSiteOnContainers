namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IGameShopItemService
{
    Task<int> CountGameShopItemAsync();
    Task<ShopItemStatusDto> CheckShopStatusAsync(int shopItemId);
    Task<List<GameShopItem>> GetGameShopItemListAsync(int pageIndex, int pageSize, int orderBy, bool isAdmin);
    Task<GameShopItem> GetGameShopItemByIdAsync(int shopItemId);
    Task<GameShopItem> GetGameShopItemByGameIdAsync(int gameInfoId);
    Task AddGameShopItemAsync(GameShopItem gameShopItem);
    Task DeleteGameShopItemByIdAsync(int shopItemId);
    Task<bool> UpdateGameShopItemInfoAsync(GameShopItem gameShopItem);
    Task TakeDownGameShopItemAsync(int shopItemId);
    Task<int> TakeUpGameShopItemAsync(int shopItemId);
    Task UpdateShopItemStockWhenTakeDownAsync(int shopItemId, int currentStock);
    Task UpdateShopItemStockWhenChangeNumberAsync(int shopItemId, int newStock);
    Task<int> UpdateShopItemInfoWhenUserBuyAsync(int shopItemId);
    Task<bool> HasSameGameShopAsync(int gameInfoId);

}