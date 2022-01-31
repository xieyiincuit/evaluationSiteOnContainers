namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IGameShopItemService
{
    Task<List<GameShopItem>> GetGameShopItemListAsync(int pageIndex, int pageSize, int orderBy);
    Task<GameShopItem> GetGameShopItemByIdAsync(int shopItemId);
    Task<GameShopItem> GetGameShopItemByGameIdAsync(int gameInfoId);
    Task<bool> AddGameShopItemAsync(GameShopItem gameShopItem);
    Task<bool> DeleteGameShopItemByIdAsync(int shopItemId);
    Task<bool> UpdateGameShopItemInfoAsync(GameShopItem gameShopItem);
    Task<bool> ChangeGameShopItemStatusAsync(int shopItemId);
    Task<int> CountGameShopItemAsync();
}