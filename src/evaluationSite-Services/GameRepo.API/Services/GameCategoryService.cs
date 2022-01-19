namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Services;

public class GameCategoryService : IGameCategory
{
    private readonly GameRepoContext _repoContext;

    public GameCategoryService(GameRepoContext repoContext)
    {
        _repoContext = repoContext;
    }

    public async Task<bool> AddCategoryAsync(GameCategory gameCategory)
    {
        await _repoContext.GameCategories.AddAsync(gameCategory);
        return await _repoContext.SaveChangesAsync() > 0;
    }

    public async Task<int> CountCategoryAsync()
    {
        return await _repoContext.GameCategories.CountAsync();
    }

    public async Task<bool> DeleteCategoryAsync(int categoryId)
    {
        var category = await _repoContext.GameCategories.FindAsync(categoryId);
        if (category == null) return false;
        _repoContext.GameCategories.Remove(category);
        return await _repoContext.SaveChangesAsync() > 0;
    }

    public async Task<List<GameCategory>> GetGameCategoriesAsync(int pageIndex, int pageSize)
    {
        var categoies = await _repoContext.GameCategories
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
        return categoies;
    }

    public async Task<GameCategory> GetGameCategoryAsync(int categoryId)
    {
        var category = await _repoContext.GameCategories
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == categoryId);
        return category;
    }

    public async Task<bool> UpdeteCategoryAsync(GameCategory gameCategory)
    {
        _repoContext.GameCategories.Update(gameCategory);
        return await _repoContext.SaveChangesAsync() > 0;
    }
}
