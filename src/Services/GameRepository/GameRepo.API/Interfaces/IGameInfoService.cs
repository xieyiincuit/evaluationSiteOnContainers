namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IGameInfoService
{
    Task<List<GameInfo>> GetGameInfosAsync(int pageIndex, int pageSize);
    Task<GameInfo> GetGameInfoAsync(int gameId);
    Task<List<GameRankDto>> GetGameInfoRankAsync();
    Task<int> CountGameInfoAsync();
    Task AddGameInfoAsync(GameInfo gameInfo);
    Task RemoveGameInfoAsync(int gameId);
    Task UpdateGameInfoAsync(GameInfo gameInfo);
    Task<bool> GameExistAsync(int gameId);
}