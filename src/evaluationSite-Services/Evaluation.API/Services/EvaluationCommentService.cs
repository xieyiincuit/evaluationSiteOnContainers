namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Services;

public class EvaluationCommentService : IEvaluationComment
{
    private readonly EvaluationContext _evaluationContext;

    public EvaluationCommentService(EvaluationContext context)
    {
        _evaluationContext = context;
    }

    public async Task<bool> AddCommentArticleAsync(EvaluationComment comment)
    {
        comment.CreateTime = DateTime.Now.ToLocalTime();
        await _evaluationContext.Comments.AddAsync(comment);
        return await _evaluationContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCommentAsync(int commentId)
    {
        var comment = await _evaluationContext.Comments.FindAsync(commentId);
        if (comment == null) return false;

        _evaluationContext.Comments.Remove(comment);
        return await _evaluationContext.SaveChangesAsync() > 0;
    }

    public async Task<int> CountArticleCommentAsync(int articleId)
    {
        return await _evaluationContext.Comments.CountAsync(comment => comment.ArticleId == articleId);
    }

    public async Task<int> CountArticleRootCommentsAsync(int articleId)
    {
        return await _evaluationContext.Comments.CountAsync(comment => comment.ArticleId == articleId && comment.IsReplay == null);
    }

    public Task<int> CountCommentChildrenCommentsAsync(int rootCommentId)
    {
        return _evaluationContext.Comments.CountAsync(comment => comment.RootCommentId == rootCommentId);
    }

    public async Task<int> CountUserCommentAsync(int userId)
    {
        return await _evaluationContext.Comments.CountAsync(x => x.UserId == userId);
    }

    public async Task<List<EvaluationComment>> GetArticleCommentsAsync(int pageIndex, int pageSize, int articleId)
    {
        //无回复状态的评论为父级评论     
        var parentComments = await _evaluationContext.Comments
            .Where(x => x.ArticleId == articleId && x.IsReplay == null)
            .OrderBy(c => c.CreateTime)
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize)
            .AsNoTracking().ToListAsync();

        return parentComments;
    }

    public async Task<List<EvaluationComment>> GetUserCommentsAsync(int pageIndex, int pageSize, int userId)
    {
        var comments = await _evaluationContext.Comments
            .Where(x => x.UserId == userId)
            .OrderBy(c => c.CreateTime)
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
        return comments;
    }

    public async Task<List<EvaluationComment>> GetCommentReplyAsync(int pageIndex, int pageSize, int parentCommentId)
    {
        var replies = await _evaluationContext.Comments.Where(rpl => rpl.IsReplay == true && rpl.RootCommentId == parentCommentId)
            .OrderBy(c => c.CreateTime)
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize)
            .AsNoTracking().ToListAsync();

        return replies;
    }

    public async Task<EvaluationComment> GetCommentById(int commentId)
    {
        return await _evaluationContext.Comments
            .Include(x => x.EvaluationArticle)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CommentId == commentId);
    }
}
