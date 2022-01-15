namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Interfaces;

public interface IEvaluationArticle
{
    Task<long> CountArticlesAsync();
    Task<long> CountArticlesByTypeAsync(int categoryId);
    Task<List<EvaluationArticle>> GetArticlesAsync(int pageSize, int pageIndex, string ids = null);
    Task<List<EvaluationArticle>> GetArticlesAsync(int pageSize, int pageIndex, int categoryTypeId);
    Task<EvaluationArticle> GetArticleAsync(int id);
    Task<bool> IsArticleExist(int id);
    Task<bool> AddArticleAsync(EvaluationArticle article);
    Task<bool> DeleteArticleAsync(int id);
    Task<bool> UpdateArticleAsync(EvaluationArticle article);
}
