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

    public async Task<List<ArticleCommentDto>> GetArticleCommentsAsync(int pageIndex, int pageSize, int articleId)
    {
        //无回复状态的评论为父级评论     
        var parentComments = await _evaluationContext.Comments
            .Where(x => x.ArticleId == articleId && x.IsReplay == null)
            .OrderBy(c => c.CreateTime)
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize)
            .AsNoTracking().ToListAsync();

        var childrenReplies = new List<EvaluationComment>();

        foreach (var parentComment in parentComments)
        {
            //分页查询每个父评论的最新五个评论
            //TODO 评论数量多后的分页查询API提供
            childrenReplies.AddRange(await FindChildrenReplyAsync(1, 5, parentComment.CommentId));
        }

        //构建结果返回
        var resultDto = new List<ArticleCommentDto>();

        //在内存中进行评论分组在返回数据
        foreach (var item in parentComments)
        {
            //Mapping Dto
            var commentsDto = _mapper.Map<ArticleCommentDto>(item);
            var childrenCommentDto = _mapper.Map<List<ReplyCommentDto>>(childrenReplies.Where(x => x.RootCommentId == item.CommentId).ToList());
            //Add to result for return
            commentsDto.Replies.AddRange(childrenCommentDto);
            resultDto.Add(commentsDto);
        }

        return resultDto;
    }

    public async Task<List<EvaluationComment>> GetUserCommentsAsync(int userId)
    {
        //TODO 待Identity服务接入后在完善
        var comments = await _evaluationContext.Comments.Where(x => x.UserId == userId).AsNoTracking().ToListAsync();
        return comments;
    }


    private async Task<List<EvaluationComment>> FindChildrenReplyAsync(int pageIndex, int pageSize, int parentCommentId)
    {
        var replies = await _evaluationContext.Comments.Where(rpl => rpl.IsReplay == true && rpl.RootCommentId == parentCommentId)
            .OrderBy(c => c.CreateTime)
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize)
            .AsNoTracking().ToListAsync();

        return replies;
    }
}
