namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IGameTag
{
    Task<List<GameTag>> GetGameTagsAsync();
    Task<GameTag> GetGameTagAsync(int tagId);
    Task<int> CountTagsAsync();
    Task<bool> AddGameTagAsync(GameTag gameTag);
    Task<bool> DeleteGameTagAsync(int tagId);
    Task<bool> UpdateGameTagAsync(GameTag gameTag);
}
