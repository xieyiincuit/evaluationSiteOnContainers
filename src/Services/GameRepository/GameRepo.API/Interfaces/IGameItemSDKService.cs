namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IGameItemSDKService
{
    /// <summary>
    /// 获取SDK根据特定的游戏发售商品
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">页大小</param>
    /// <param name="gameItemId">游戏发售商品Id</param>
    /// <param name="hasSend">是否已经发出</param>
    /// <returns></returns>
    Task<List<GameItemSDK>> GetSDKListByGameItemAsync(int pageIndex, int pageSize, int gameItemId, bool? hasSend = false);

    Task<long> CountSDKNumberByGameItemOrStatusAsync(int gameItemId, bool? hasSend = false);

    Task<bool> GenerateSDKForGameShopItemAsync(int count, int gameItemId);

    Task<int> BatchUpdateSDKStatusAsync(List<int> sdkIds);

    Task<int> BatchDeleteGameItemsSDKAsync(int gameItemId, bool? hasSend = false);

    Task<GameItemSDK> GetOneSDKToSendUserAsync(int shopItemId);
}