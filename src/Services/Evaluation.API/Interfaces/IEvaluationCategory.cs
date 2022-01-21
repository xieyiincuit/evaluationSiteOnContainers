namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Interfaces;

public interface IEvaluationCategory
{
    Task<List<EvaluationCategory>> GetEvaluationCategoriesAsync();
    Task<EvaluationCategory> GetEvaluationCategoryAsync(int categoryId);
    Task<bool> AddEvaluationCategoryAsync(EvaluationCategory category);
    Task<bool> UpdateEvaluationCategoryAsync(EvaluationCategory category);
    Task<bool> DeleteEvaluationCategoryAsync(int categoryId);
}