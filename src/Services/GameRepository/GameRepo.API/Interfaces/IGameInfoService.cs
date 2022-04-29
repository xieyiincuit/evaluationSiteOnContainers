namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IGameInfoService
{
    Task<List<GameInfo>> GetGameInfosAsync(int pageIndex, int pageSize);
    Task<List<GameInfo>> GetGameInfoWithTermAsync(int pageIndex, int pageSize, int? categoryId, int? companyId, string? order);
    Task<GameInfo> GetGameInfoAsync(int gameId);
    Task<List<GameRankDto>> GetGameInfoRankAsync();
    Task<List<GameSelectDto>> GetGameSelectAsync();
    Task<int> CountGameInfoAsync();
    Task AddGameInfoAsync(GameInfo gameInfo);
    Task RemoveGameInfoAsync(int gameId);
    Task UpdateGameInfoAsync(GameInfo gameInfo);
    Task<bool> GameExistAsync(int gameId);
    Task<bool> HasSameGameNameAsync(string gameName);
    Task UpdateGameInfoWhenUserBuyAsync(int gameId);
}