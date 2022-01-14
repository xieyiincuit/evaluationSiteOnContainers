namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Services;

public class EvaluationCommentService : IEvaluationComment
{
    private readonly EvaluationContext _evaluationContext;

    public EvaluationCommentService(EvaluationContext context)
    {
        _evaluationContext = context;
    }

    public Task<List<EvaluationComment>> GetArticleComments(int articleId)
    {
        var comments = _evaluationContext.Comments.Where(x=>x.ArticleId == articleId).AsNoTracking().ToListAsync();
        return comments;
    }

    public Task<List<EvaluationComment>> GetUserComments(int userId)
    {
        var comments = _evaluationContext.Comments.Where(x => x.UserId == userId).AsNoTracking().ToListAsync();
        return comments;
    }
}
