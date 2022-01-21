namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Controllers;

[Route("api/v1/e")]
[ApiController]
public class EvaluationCommentController : ControllerBase
{
    private const int _pageSize = 5;
    private readonly IEvaluationArticle _articleService;
    private readonly IEvaluationComment _commentService;
    private readonly IMapper _mapper;

    private readonly Dictionary<int, string> _userDic = new()
    {
        {1, "Zhousl"},
        {2, "Hanby"},
        {3, "Chenxy"},
        {4, "Wangxb"},
        {5, "Lvcf"}
    };

    public EvaluationCommentController(IEvaluationArticle articleService, IEvaluationComment commentService,
        IMapper mapper)
    {
        _articleService = articleService;
        _commentService = commentService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("article/{articleId:int}/comments")]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<ArticleCommentDto>), (int) HttpStatusCode.OK)]
    public async Task<IActionResult> GetArticleComments([FromRoute] int articleId, [FromQuery] int pageIndex = 1)
    {
        if (articleId <= 0 || articleId >= int.MaxValue) return BadRequest(); // id非法
        if (await _articleService.IsArticleExist(articleId) == false) return NotFound(); //文章不存在

        var totalComments = await _commentService.CountArticleRootCommentsAsync(articleId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalComments, _pageSize, pageIndex)) pageIndex = 1;

        //分页获取文章父级评论
        var parentComments = await _commentService.GetArticleCommentsAsync(pageIndex, _pageSize, articleId);

        var childrenReplies = new List<EvaluationComment>();
        foreach (var parentComment in parentComments)
            //分页查询每个父评论的最新五个评论
            //TODO 评论数量多后的分页查询API提供
            childrenReplies.AddRange(await _commentService.GetCommentReplyAsync(1, 5, parentComment.CommentId));

        //构建结果返回
        var resultDto = new List<ArticleCommentDto>();
        //在内存中进行评论分组在返回数据
        foreach (var item in parentComments)
        {
            var commentsDto = _mapper.Map<ArticleCommentDto>(item);
            var childrenCommentDto =
                _mapper.Map<List<ReplyCommentDto>>(childrenReplies.Where(x => x.RootCommentId == item.CommentId)
                    .ToList());

            commentsDto.Replies.AddRange(childrenCommentDto);
            resultDto.Add(commentsDto);
        }

        var model = new PaginatedItemsDtoModel<ArticleCommentDto>(pageIndex, _pageSize, totalComments, resultDto);
        return Ok(model);
    }

    [HttpGet]
    [Route("u/{userId:int}/comments")]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserComments([FromRoute] int userId, [FromQuery] int pageIndex = 1)
    {
        if (userId <= 0 || userId >= int.MaxValue) return BadRequest();

        var totalComments = await _commentService.CountUserCommentAsync(userId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalComments, _pageSize, pageIndex)) pageIndex = 1;

        var comments = await _commentService.GetUserCommentsAsync(pageIndex, _pageSize, userId);
        return Ok(comments);
    }

    [HttpGet("comments/{commentId:int}", Name = nameof(GetCommentByIdAsync))]
    //[Route("comments/{commentId:int}")]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(EvaluationComment), (int) HttpStatusCode.OK)]
    public async Task<ActionResult<EvaluationComment>> GetCommentByIdAsync([FromRoute] int commentId)
    {
        if (commentId <= 0 || commentId >= int.MaxValue) return BadRequest();

        var comment = await _commentService.GetCommentById(commentId);
        if (comment == null) return NotFound();
        return Ok(comment);
    }

    [HttpPost]
    [Route("article/comments")]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    [ProducesResponseType((int) HttpStatusCode.Created)]
    public async Task<IActionResult> PostCommentOnArticleAsync([FromBody] ArticleCommentAddDto commentAddDto)
    {
        if (commentAddDto == null) return BadRequest();
        if (!await _articleService.IsArticleExist(commentAddDto.ArticleId)) return BadRequest();

        var comment = _mapper.Map<EvaluationComment>(commentAddDto);

        //TODO Get real userInfo
        comment.UserId = 1;
        comment.NickName = _userDic[comment.UserId];

        await _commentService.AddCommentArticleAsync(comment);
        return CreatedAtRoute(nameof(GetCommentByIdAsync), new {commentId = comment.CommentId}, null);
    }

    [HttpPost]
    [Route("article/comments/reply")]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    [ProducesResponseType((int) HttpStatusCode.Created)]
    public async Task<IActionResult> PostReplyOnCommentAsync([FromBody] ReplyCommentAddDto replyAddDto)
    {
        if (replyAddDto == null) return BadRequest();
        if (!await _articleService.IsArticleExist(replyAddDto.ArticleId)) return BadRequest();

        var comment = _mapper.Map<EvaluationComment>(replyAddDto);

        comment.UserId = 1;
        comment.NickName = _userDic[comment.UserId];
        comment.IsReplay = true;
        comment.ReplyNickName = _userDic[comment.ReplyUserId.Value];

        await _commentService.AddCommentArticleAsync(comment);
        return CreatedAtRoute(nameof(GetCommentByIdAsync), new {commentId = comment.CommentId}, null);
    }

    [HttpDelete]
    [Route("article/comments/{commentId:int}")]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCommentAsync([FromRoute] int commentId)
    {
        throw new NotImplementedException();
    }
}