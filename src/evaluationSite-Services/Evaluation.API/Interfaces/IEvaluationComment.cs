namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Interfaces;

public interface IEvaluationComment
{
    Task<List<ArticleCommentDto>> GetArticleCommentsAsync(int pageIndex, int pageSize, int articleId);
    Task<List<EvaluationComment>> GetUserCommentsAsync(int userId);

    Task<int> CountArticleCommentAsync(int articleId);
    Task<int> CountArticleRootCommentsAsync(int articleId);
    Task<int> CountCommentChildrenCommentsAsync(int rootCommentId);
}
