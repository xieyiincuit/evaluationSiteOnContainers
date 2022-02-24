namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IGameCategoryService
{
    Task<List<GameCategory>> GetGameCategoriesAsync(int pageIndex, int pageSize);
    Task<GameCategory> GetGameCategoryAsync(int categoryId);
    Task<int> CountCategoryAsync();
    Task<bool> AddCategoryAsync(GameCategory gameCategory);
    Task<bool> UpdateCategoryAsync(GameCategory gameCategory);
    Task<bool> DeleteCategoryAsync(int categoryId);
}