namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Services;

public class EvaluationCategoryService : IEvaluationCategory
{
    private readonly EvaluationContext _evaluationContext;

    public EvaluationCategoryService(EvaluationContext context)
    {
        _evaluationContext = context;
    }

    public async Task<List<EvaluationCategory>> GetEvaluationCategoriesAsync()
    {
        var category = await _evaluationContext.Categories.AsNoTracking().ToListAsync();
        return category;
    }
}
