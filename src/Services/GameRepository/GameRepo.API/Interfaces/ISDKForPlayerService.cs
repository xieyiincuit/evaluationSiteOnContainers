namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface ISDKForPlayerService
{
    Task<List<GameSDKForPlayer>> GetPlayerSDKByUserIdAsync(string userId);
    Task<List<GameSDKForPlayer>> GetPlayerSDKByUserIdAndStatusAsync(string userId, bool? hasChecked = false);
    Task<GameSDKForPlayer> GetPlayerSDKByIdAsync(int id);
    Task<bool> UpdatePlayerSDKStatusCheck(int id);
}