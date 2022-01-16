namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Interfaces;

public interface IEvaluationComment
{
    Task<List<EvaluationComment>> GetArticleCommentsAsync(int pageIndex, int pageSize, int articleId);
    Task<List<EvaluationComment>> GetCommentReplyAsync(int pageIndex, int pageSize, int parentCommentId);
    Task<List<EvaluationComment>> GetUserCommentsAsync(int pageIndex, int pageSize, int userId);
    Task<EvaluationComment> GetCommentById(int commentId);
    Task<bool> AddCommentArticleAsync(EvaluationComment comment);
    Task<bool> DeleteCommentAsync(int commentId);
    Task<int> CountArticleCommentAsync(int articleId);
    Task<int> CountUserCommentAsync(int userId);
    Task<int> CountArticleRootCommentsAsync(int articleId);
    Task<int> CountCommentChildrenCommentsAsync(int rootCommentId);
}
