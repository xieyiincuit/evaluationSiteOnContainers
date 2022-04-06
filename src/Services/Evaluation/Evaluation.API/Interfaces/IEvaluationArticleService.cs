namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Interfaces;

public interface IEvaluationArticleService
{
    Task<int> CountArticlesAsync();
    Task<int> CountArticlesByTypeAsync(int categoryId);
    Task<int> CountArticlesByUserAsync(string userId);
    Task<int> CountArticlesByGameAsync(int gameId);
    Task<List<ArticleSmallDto>> GetArticlesAsync(int pageSize, int pageIndex);
    Task<List<ArticleSmallDto>> GetArticlesAsync(int pageSize, int pageIndex, int categoryTypeId);
    Task<List<ArticleGameDto>> GetArticlesByGameAsync(int pageSize, int pageIndex, int gameId);
    Task<ArticleShopDto> GetArticlesByShopItemAsync(int gameId);
    Task<List<ArticleTableDto>> GetUserArticlesAsync(int pageSize, int pageIndex, string userId, int? categoryId = null, bool timeDesc = true);
    Task<EvaluationArticle> GetArticleAsync(int id);
    Task<bool> IsArticleExist(int id);
    Task<bool> AddArticleAsync(EvaluationArticle article);
    Task<bool> DeleteArticleAsync(int id);
    Task<bool> UpdateArticleAsync(EvaluationArticle article);
    Task<List<EvaluationArticle>> GetArticlesByGameInfoAsync(int gameId);
    Task<List<EvaluationArticle>> GetArticlesByAuthorInfoAsync(string userId);
    Task<bool> BatchUpdateArticlesAsync();
}