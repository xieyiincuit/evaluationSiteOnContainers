namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Controllers;

[Route("api/v1")]
[ApiController]
public class EvaluationCommentController : ControllerBase
{
    private const int _pageSize = 5;
    private readonly IEvaluationArticleService _articleService;
    private readonly IEvaluationCommentService _commentService;
    private readonly IdentityCallService _identityService;
    private readonly ILogger<EvaluationCommentController> _logger;
    private readonly IMapper _mapper;

    public EvaluationCommentController(
        IEvaluationArticleService articleService,
        IEvaluationCommentService commentService,
        IMapper mapper,
        ILogger<EvaluationCommentController> logger,
        IdentityCallService identityService)
    {
        _articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
        _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
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

        var userIds = new HashSet<string>();

        //分页获取文章父级评论
        var parentComments = await _commentService.GetArticleCommentsAsync(pageIndex, _pageSize, articleId);

        //评论回复
        var childrenReplies = new List<EvaluationComment>();
        foreach (var parentComment in parentComments)
        {
            //默认查询每个父评论的最新五个评论
            childrenReplies.AddRange(await _commentService.GetCommentReplyAsync(1, 5, parentComment.CommentId));
            //记录参与评论的用户Id
            userIds.Add(parentComment.UserId);
        }

        //构建结果返回
        var resultDto = new List<ArticleCommentDto>();
        //在内存中进行评论分组在返回数据
        foreach (var item in parentComments)
        {
            var commentsDto = _mapper.Map<ArticleCommentDto>(item);

            var childrenCommentDto = _mapper.Map<List<ReplyCommentDto>>(
                childrenReplies.Where(x => x.RootCommentId == item.CommentId).ToList());

            //记录参与评论的用户Id
            childrenCommentDto.ForEach(x => userIds.Add(x.UserId));
            commentsDto.Replies.AddRange(childrenCommentDto);
            resultDto.Add(commentsDto);
        }

        //调用链的最上方释放
        using var response = await _identityService.GetCommentsUserProfileAsync(userIds.ToList());
        var userInfoDto = new List<UserAvatarDto>();
        if (response.IsSuccessStatusCode) userInfoDto = await response.Content.ReadFromJsonAsync<List<UserAvatarDto>>();
        var model = new PaginatedItemsDtoModel<ArticleCommentDto>(pageIndex, _pageSize, totalComments, resultDto,
            userInfoDto);
        return Ok(model);
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("article/comments/{rootCommentId:int}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<ReplyCommentDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetRootCommentsReplies([FromRoute] int rootCommentId,
        [FromQuery] int pageIndex = 1)
    {
        if (rootCommentId <= 0 || rootCommentId >= int.MaxValue) return BadRequest(); // id非法

        var parentComment = await _commentService.GetCommentById(rootCommentId);

        if (parentComment == null) return NotFound(); //父评论不存在

        var userIds = new HashSet<string> { parentComment.UserId };

        var totalComments = await _commentService.CountCommentChildrenCommentsAsync(rootCommentId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalComments, _pageSize, pageIndex)) pageIndex = 1;

        var childrenReplies = new List<EvaluationComment>();
        childrenReplies.AddRange(
            await _commentService.GetCommentReplyAsync(pageIndex, _pageSize, parentComment.CommentId));

        var childrenCommentDto = _mapper.Map<List<ReplyCommentDto>>(childrenReplies);
        childrenCommentDto.ForEach(x => userIds.Add(x.UserId));

        using var response = await _identityService.GetCommentsUserProfileAsync(userIds.ToList());
        var userInfoDto = new List<UserAvatarDto>();
        if (response.IsSuccessStatusCode) userInfoDto = await response.Content.ReadFromJsonAsync<List<UserAvatarDto>>();
        var model = new PaginatedItemsDtoModel<ReplyCommentDto>(pageIndex, _pageSize, totalComments, childrenCommentDto,
            userInfoDto);
        return Ok(model);
    }

    [Authorize]
    [HttpGet]
    [Route("comments/user")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserComments([FromQuery] int pageIndex = 1)
    {
        var loginUserId = User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(loginUserId)) return BadRequest();

        var totalComments = await _commentService.CountUserCommentAsync(loginUserId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalComments, _pageSize, pageIndex)) pageIndex = 1;

        var comments = await _commentService.GetUserCommentsAsync(pageIndex, _pageSize, loginUserId);
        return Ok(comments);
    }

    [AllowAnonymous]
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

        var loginUserId = User.FindFirstValue("sub");
        comment.UserId = loginUserId;

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

        var loginUserId = User.FindFirstValue("sub");
        comment.UserId = loginUserId;
        comment.IsReplay = true;

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