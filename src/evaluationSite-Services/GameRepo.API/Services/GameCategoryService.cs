namespace GameRepo.API.Services;

public class GameCategoryService : IGameCategory
{
    public GameCategoryService()
    {

    }

    public async Task<bool> AddCategoryAsync(GameCategory gameCategory)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountCategoryAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteCategoryAsync(int categoryId)
    {
        throw new NotImplementedException();
    }

    public Task<List<GameCategory>> GetGameCategoriesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<GameCategory> GetGameCategoryAsync(int categoryId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdeteCategoryAsync(GameCategory gameCategory)
    {
        throw new NotImplementedException();
    }
}
