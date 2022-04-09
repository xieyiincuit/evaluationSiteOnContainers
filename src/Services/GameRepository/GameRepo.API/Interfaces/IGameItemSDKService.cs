namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IGameItemSDKService
{
    Task<List<GameItemSDK>> GetSDKListByGameItemAsync(int pageIndex, int pageSize, int shopItemId, bool? hasSend = false);

    Task<long> CountSDKNumberByGameItemOrStatusAsync(int shopItemId, bool? hasSend);

    Task GenerateSDKForGameShopItemAsync(int count, int shopItemId);

    Task<int> BatchUpdateSDKStatusAsync(List<int> sdkIds);

    Task BatchDeleteGameItemsSDKAsync(int shopItemId, bool? hasSend, int deleteCount);

    Task<GameItemSDK> GetOneSDKToSendUserAsync(int shopItemId);
}