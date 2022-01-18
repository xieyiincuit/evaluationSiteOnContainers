namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IGameCategory
{
    Task<List<GameCategory>> GetGameCategoriesAsync();
    Task<GameCategory> GetGameCategoryAsync(int categoryId);
    Task<int> CountCategoryAsync();
    Task<bool> AddCategoryAsync(GameCategory gameCategory);
    Task<bool> UpdeteCategoryAsync(GameCategory gameCategory);
    Task<bool> DeleteCategoryAsync(int categoryId);
}
