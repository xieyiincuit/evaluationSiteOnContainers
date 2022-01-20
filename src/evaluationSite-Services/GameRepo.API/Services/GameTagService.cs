namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class GameTagService : IGameTagService
{
    private readonly GameRepoContext _repoContext;

    public GameTagService(GameRepoContext repoContext)
    {
        _repoContext = repoContext;
    }

    public async Task<bool> AddGameTagAsync(GameTag gameTag)
    {
        await _repoContext.GameTags.AddAsync(gameTag);
        return await _repoContext.SaveChangesAsync() > 0;
    }

    public async Task<int> CountTagsAsync()
    {
        return await _repoContext.GameTags.CountAsync();
    }

    public async Task<bool> DeleteGameTagAsync(int tagId)
    {
        var tag = await _repoContext.GameTags.FindAsync(tagId);
        if (tag == null) return false;
        _repoContext.GameTags.Remove(tag);
        return await _repoContext.SaveChangesAsync() > 0;
    }

    public async Task<List<GameCategory>> GetGameCategoriesAsync()
    {
        var categoies = await _repoContext.GameCategories
            .OrderBy(x => x.CategoryName)
            .AsNoTracking()
            .ToListAsync();
        return categoies;
    }

    public async Task<GameTag> GetGameTagAsync(int tagId)
    {
        var tag = await _repoContext.GameTags.FindAsync(tagId);
        return tag;
    }

    public async Task<List<GameTag>> GetGameTagsAsync(int pageIndex, int pageSize)
    {
        var tags = await _repoContext.GameTags
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
        return tags;
    }

    public async Task<bool> UpdateGameTagAsync(GameTag gameTag)
    {
        _repoContext.GameTags.Update(gameTag);
        return await _repoContext.SaveChangesAsync() > 0;
    }
}
