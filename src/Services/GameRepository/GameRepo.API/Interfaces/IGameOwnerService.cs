namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IGameOwnerService
{
    Task<GameOwner> GetGameOwnerRecordAsync(string userId, int gameId);
    Task<bool> DeleteGameOwnerRecordAsync(string userId, int gameId);
    Task AddGameOwnerRecordAsync(string userId, int gameId);
    Task UpdateGameScoreAsync(string userId, int gameId, double gameScore);

    Task<double> CalculateGameScore(int gameId);
}