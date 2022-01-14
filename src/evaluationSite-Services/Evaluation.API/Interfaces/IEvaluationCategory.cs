namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Interfaces;

public interface IEvaluationCategory
{
    Task<List<EvaluationCategory>> GetEvaluationCategoriesAsync();
}
