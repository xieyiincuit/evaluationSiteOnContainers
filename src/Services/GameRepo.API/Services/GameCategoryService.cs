namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class GameCategoryService : IGameCategoryService
{
    private readonly GameRepoContext _repoContext;

    public GameCategoryService(GameRepoContext repoContext)
    {
        _repoContext = repoContext ?? throw new ArgumentNullException(nameof(repoContext));
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
        var categories = await _repoContext.GameCategories
            .OrderBy(x => x.CategoryName)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
        return categories;
    }

    public async Task<GameCategory> GetGameCategoryAsync(int categoryId)
    {
        var category = await _repoContext.GameCategories
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == categoryId);
        return category;
    }

    public async Task<bool> UpdateCategoryAsync(GameCategory gameCategory)
    {
        _repoContext.GameCategories.Update(gameCategory);
        return await _repoContext.SaveChangesAsync() > 0;
    }
}
