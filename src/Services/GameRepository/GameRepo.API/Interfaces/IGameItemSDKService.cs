namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IGameItemSDKService
{
    Task<List<GameItemSDK>> GetSDKListByGameItemAsync(int pageIndex, int pageSize, int gameItemId, bool? hasSend = false);

    Task<long> CountSDKNumberByGameItemOrStatusAsync(int gameItemId, bool? hasSend);

    Task GenerateSDKForGameShopItemAsync(int count, int gameItemId);

    Task<int> BatchUpdateSDKStatusAsync(List<int> sdkIds);

    Task BatchDeleteGameItemsSDKAsync(int gameItemId, bool? hasSend, int deleteCount);

    Task<GameItemSDK> GetOneSDKToSendUserAsync(int shopItemId);
}