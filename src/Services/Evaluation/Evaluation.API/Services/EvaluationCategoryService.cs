namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Services;

public class EvaluationCategoryService : IEvaluationCategoryService
{
    private readonly EvaluationContext _evaluationContext;

    public EvaluationCategoryService(EvaluationContext context)
    {
        _evaluationContext = context;
    }

    public async Task<bool> AddEvaluationCategoryAsync(EvaluationCategory category)
    {
        await _evaluationContext.Categories.AddAsync(category);
        return await _evaluationContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteEvaluationCategoryAsync(int categoryId)
    {
        var category = await _evaluationContext.Categories.FindAsync(categoryId);
        if (category == null) return false;
        _evaluationContext.Categories.Remove(category);
        return await _evaluationContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> HasSameCategoryNameAsync(string categoryName)
    {
        var category = await _evaluationContext.Categories.FirstOrDefaultAsync(x => x.CategoryType == categoryName);
        return category != null;
    }

    public async Task<List<EvaluationCategory>> GetEvaluationCategoriesAsync()
    {
        var category = await _evaluationContext.Categories.AsNoTracking().ToListAsync();
        return category;
    }

    public async Task<EvaluationCategory> GetEvaluationCategoryAsync(int categoryId)
    {
        return await _evaluationContext.Categories.FindAsync(categoryId);
    }

    public async Task<bool> UpdateEvaluationCategoryAsync(EvaluationCategory category)
    {
        _evaluationContext.Categories.Update(category);
        return await _evaluationContext.SaveChangesAsync() > 0;
    }
}