namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Interfaces;
public interface IGameInfo
{
    Task<List<GameInfo>> GetGameInfosAsync(int pageIndex, int pageSize);
    Task<GameInfo> GetGameInfoAsync(int gameId);
    Task<int> CountGameInfoAsync();
    Task<bool> AddGameInfoAsync(GameInfo gameInfo);
    Task<bool> RemoveGameInfoAsync(int gameId);
    Task<bool> UpdateGameInfoAsync(GameInfo gameInfo);
}
