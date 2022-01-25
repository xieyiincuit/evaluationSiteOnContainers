namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Controllers;

[Route("api/v1/e")]
[ApiController]
public class EvaluationCommentController : ControllerBase
{
    private const int _pageSize = 5;
    private readonly IEvaluationArticleService _articleService;
    private readonly IEvaluationCommentService _commentService;
    private readonly IMapper _mapper;
    private readonly ILogger<EvaluationCommentController> _logger;

    public EvaluationCommentController(
        IEvaluationArticleService articleService, 
        IEvaluationCommentService commentService,
        IMapper mapper,
        ILogger<EvaluationCommentController> logger)
    {
        _articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
        _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("article/{articleId:int}/comments")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<ArticleCommentDto>), (int)HttpStatusCode.OK)]
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

    [Authorize]
    [HttpGet]
    [Route("u/comments")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserComments([FromQuery] int pageIndex = 1)
    {
        var loginUserId = User.FindFirst("sub").Value;
        if (string.IsNullOrEmpty(loginUserId)) return BadRequest();

        var totalComments = await _commentService.CountUserCommentAsync(loginUserId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalComments, _pageSize, pageIndex)) pageIndex = 1;

        var comments = await _commentService.GetUserCommentsAsync(pageIndex, _pageSize, loginUserId);
        return Ok(comments);
    }

    [HttpGet("comments/{commentId:int}", Name = nameof(GetCommentByIdAsync))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(EvaluationComment), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<EvaluationComment>> GetCommentByIdAsync([FromRoute] int commentId)
    {
        if (commentId <= 0 || commentId >= int.MaxValue) return BadRequest();

        var comment = await _commentService.GetCommentById(commentId);
        if (comment == null) return NotFound();
        return Ok(comment);
    }

    [HttpPost]
    [Authorize(Roles = "administrator, evaluator, normaluser")]
    [Route("article/comments")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> PostCommentOnArticleAsync([FromBody] ArticleCommentAddDto commentAddDto)
    {
        if (commentAddDto == null) return BadRequest();
        if (!await _articleService.IsArticleExist(commentAddDto.ArticleId)) return BadRequest();

        var comment = _mapper.Map<EvaluationComment>(commentAddDto);

        var loginUserId = User.FindFirst("sub").Value;
        comment.UserId = loginUserId;
        comment.NickName = User.Identity.Name;

        await _commentService.AddCommentArticleAsync(comment);
        return CreatedAtRoute(nameof(GetCommentByIdAsync), new { commentId = comment.CommentId }, null);
    }

    [HttpPost]
    [Authorize(Roles = "administrator, evaluator, normaluser")]
    [Route("article/comments/reply")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> PostReplyOnCommentAsync([FromBody] ReplyCommentAddDto replyAddDto)
    {
        if (replyAddDto == null) return BadRequest();
        if (!await _articleService.IsArticleExist(replyAddDto.ArticleId)) return BadRequest();

        var comment = _mapper.Map<EvaluationComment>(replyAddDto);

        var loginUserId = User.FindFirst("sub").Value;
        comment.UserId = loginUserId;
        comment.NickName = User.Identity.Name;
        comment.IsReplay = true;
        comment.ReplyNickName = replyAddDto.RelayUserName;

        await _commentService.AddCommentArticleAsync(comment);
        return CreatedAtRoute(nameof(GetCommentByIdAsync), new { commentId = comment.CommentId }, null);
    }

    [HttpDelete]
    [Authorize(Roles = "administrator, evaluator")]
    [Route("article/comments/{commentId:int}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCommentAsync([FromRoute] int commentId)
    {
        throw new NotImplementedException();
    }
}