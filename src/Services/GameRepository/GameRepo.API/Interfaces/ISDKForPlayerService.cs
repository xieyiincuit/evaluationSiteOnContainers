namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface ISDKForPlayerService
{
    Task<List<PlaySDKDto>> GetPlayerSDKByUserIdAsync(string userId, int pageSize, int pageIndex);
    Task<List<PlaySDKDto>> GetPlayerSDKByUserIdAndStatusAsync(string userId, int pageSize, int pageIndex, bool? hasChecked = false);
    Task<GameSDKForPlayer> GetPlayerSDKByIdAsync(int id);
    Task<bool> UpdatePlayerSDKStatusCheck(int id);
}