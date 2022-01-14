namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Services;

public class EvaluationCommentService : IEvaluationComment
{
    private readonly EvaluationContext _evaluationContext;
    private readonly IMapper _mapper;

    public EvaluationCommentService(EvaluationContext context, IMapper mapper)
    {
        _evaluationContext = context;
        _mapper = mapper;
    }

    public async Task<List<EvaluationComment>> GetArticleComments(int pageIndex, int pageSize, int articleId)
    {
        //无回复状态的评论为父级评论
        //评论也采用分页返回
        var parentComments = await _evaluationContext.Comments
            .Where(x => x.ArticleId == articleId && x.IsReplay == false)
            .OrderBy(c => c.CreateTime)
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize)
            .AsNoTracking().ToListAsync();

        var childrenReplies = new List<EvaluationComment>();

        //并发处理 减少数据库RT时间
        Parallel.ForEach(parentComments, async x =>
        {
            childrenReplies.AddRange(await FindChildrenReplyAsync(1, 5, x.CommentId));
        });

        //在内存中进行评论分组在返回数据
        foreach (var item in parentComments)
        {
            var commentsDto = _mapper.Map<ArticleCommentDto>(item);
            var childrenCommentDto = _mapper.Map<List<ArticleCommentDto>>(childrenReplies.Where(x => x.ReplayCommentId == item.CommentId).ToList());         
        }

        //TODO 递归处理回复评论
        return parentComments;
    }

    public async Task<List<EvaluationComment>> GetUserComments(int userId)
    {
        //TODO 待Identity服务接入后在完善
        var comments = await _evaluationContext.Comments.Where(x => x.UserId == userId).AsNoTracking().ToListAsync();
        return comments;
    }

    private async Task<List<EvaluationComment>> FindChildrenReplyAsync(int pageIndex, int pageSize, int parentCommentId)
    {
        var replies = await _evaluationContext.Comments.Where(rpl => rpl.IsReplay == true && rpl.ReplayCommentId == parentCommentId)
            .OrderBy(c => c.CreateTime)
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize)
            .AsNoTracking().ToListAsync();

        return replies;
    }


}
