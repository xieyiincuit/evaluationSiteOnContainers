namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Controllers;

/// <summary>
/// 测评文章评论接口
/// </summary>
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

    /// <summary>
    /// 用户——获取测评文章的评论内容
    /// </summary>
    /// <param name="articleId">文章Id</param>
    /// <param name="pageIndex">pageSize=5</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("article/{articleId:int}/comments")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(PaginatedItemsWithUserDtoModel<ArticleCommentDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetArticleComments([FromRoute] int articleId, [FromQuery] int pageIndex = 1)
    {
        if (articleId <= 0 || articleId >= int.MaxValue) return BadRequest(); // id非法
        if (await _articleService.IsArticleExist(articleId) == false) return BadRequest(); //文章不存在

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
            commentsDto.RepliesCount = await _commentService.CountCommentChildrenCommentsAsync(item.CommentId);
            var childrenCommentDto = _mapper.Map<List<ReplyCommentDto>>(childrenReplies.Where(x => x.RootCommentId == item.CommentId).ToList());

            //记录参与评论的用户Id
            childrenCommentDto.ForEach(x => userIds.Add(x.UserId));
            commentsDto.Replies.AddRange(childrenCommentDto);
            resultDto.Add(commentsDto);
        }

        //获取评论的用户额外信息
        using var response = await _identityService.GetCommentsUserProfileAsync(userIds.ToList());
        var userInfoDto = new List<UserAvatarDto>();
        if (response.IsSuccessStatusCode) userInfoDto = await response.Content.ReadFromJsonAsync<List<UserAvatarDto>>();

        var model = new PaginatedItemsWithUserDtoModel<ArticleCommentDto>(pageIndex, _pageSize, totalComments, resultDto, userInfoDto);
        return Ok(model);
    }

    /// <summary>
    /// 用户——获取父级评论的子评论列表
    /// </summary>
    /// <param name="rootCommentId">父级评论Id</param>
    /// <param name="pageIndex">pageSize=5</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("article/comments/{rootCommentId:int}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(PaginatedItemsWithUserDtoModel<ReplyCommentDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetRootCommentsReplies([FromRoute] int rootCommentId,
        [FromQuery] int pageIndex = 1)
    {
        if (rootCommentId <= 0 || rootCommentId >= int.MaxValue) return BadRequest(); // id非法

        var parentComment = await _commentService.GetCommentById(rootCommentId);
        if (parentComment == null) return NotFound(); //父评论不存在

        var totalComments = await _commentService.CountCommentChildrenCommentsAsync(rootCommentId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalComments, _pageSize, pageIndex)) pageIndex = 1;

        var childrenReplies = new List<EvaluationComment>();
        childrenReplies.AddRange(await _commentService.GetCommentReplyAsync(pageIndex, _pageSize, parentComment.CommentId));

        // 暂存该评论的所有参与人员的Id
        var userIds = new HashSet<string> { parentComment.UserId };
        var childrenCommentDto = _mapper.Map<List<ReplyCommentDto>>(childrenReplies);
        childrenCommentDto.ForEach(x => userIds.Add(x.UserId));

        // 获取用户额外信息
        using var response = await _identityService.GetCommentsUserProfileAsync(userIds.ToList());
        var userInfoDto = new List<UserAvatarDto>();
        if (response.IsSuccessStatusCode) userInfoDto = await response.Content.ReadFromJsonAsync<List<UserAvatarDto>>();

        var model = new PaginatedItemsWithUserDtoModel<ReplyCommentDto>(pageIndex, _pageSize, totalComments, childrenCommentDto, userInfoDto);
        return Ok(model);
    }

    /// <summary>
    /// 用户——获取自己的测评评论 (目前无需求来使用该接口)
    /// </summary>
    /// <param name="pageIndex">pageSize=5</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("comments/user")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(List<EvaluationComment>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserComments([FromQuery] int pageIndex = 1)
    {
        var loginUserId = User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(loginUserId)) return BadRequest();

        var totalComments = await _commentService.CountUserCommentAsync(loginUserId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalComments, _pageSize, pageIndex)) pageIndex = 1;

        var comments = await _commentService.GetUserCommentsAsync(pageIndex, _pageSize, loginUserId);
        return Ok(comments);
    }


    /// <summary>
    /// 用户——获取特定的评论信息
    /// </summary>
    /// <param name="commentId">评论Id</param>
    /// <returns></returns>
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

    /// <summary>
    /// 用户——在测评文章中发表评论
    /// </summary>
    /// <param name="commentAddDto"></param>
    /// <returns></returns>
    [HttpPost("article/comments")]
    [Authorize(Roles = "administrator, evaluator, normaluser")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> PostCommentOnArticleAsync([FromBody] ArticleCommentAddDto commentAddDto)
    {
        if (commentAddDto == null) return BadRequest();
        if (!await _articleService.IsArticleExist(commentAddDto.ArticleId)) return BadRequest();

        var comment = _mapper.Map<EvaluationComment>(commentAddDto);
        comment.UserId = User.FindFirstValue("sub");

        var addResponse = await _commentService.AddCommentArticleAsync(comment);
        if (addResponse == false)
        {
            _logger.LogError("user:{Userid}:{UserName} wanna to comment on article:{ArticleId}, but fail",
                User.FindFirstValue("sub"), User.Identity.Name, commentAddDto.ArticleId);
            throw new EvaluationDomainException("测评文章评论失败");
        }

        return CreatedAtRoute(nameof(GetCommentByIdAsync), new { commentId = comment.CommentId }, new { rootCommentId = comment.CommentId });
    }

    /// <summary>
    /// 用户——在测评文章中回复已发表的评论
    /// </summary>
    /// <param name="replyAddDto"></param>
    /// <returns></returns>
    /// <exception cref="EvaluationDomainException"></exception>
    [HttpPost("article/comments/reply")]
    [Authorize(Roles = "administrator, evaluator, normaluser")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> PostReplyOnCommentAsync([FromBody] ReplyCommentAddDto replyAddDto)
    {
        if (replyAddDto == null) return BadRequest();
        if (!await _articleService.IsArticleExist(replyAddDto.ArticleId)) return BadRequest();

        var comment = _mapper.Map<EvaluationComment>(replyAddDto);
        comment.UserId = User.FindFirstValue("sub");
        comment.IsReply = true;

        var addResponse = await _commentService.AddCommentArticleAsync(comment);
        if (addResponse == false)
        {
            _logger.LogError("user:{Userid}:{UserName} wanna to reply on comment:{CommentId}, but fail",
                User.FindFirstValue("sub"), User.Identity.Name, replyAddDto.RootCommentId);
            throw new EvaluationDomainException("文章评论回复失败");
        }

        return CreatedAtRoute(nameof(GetCommentByIdAsync), new { commentId = comment.CommentId }, new { subCommentId = comment.CommentId });
    }

    /// <summary>
    /// 用户——删除自己的评论 (未实现)
    /// </summary>
    /// <param name="commentId">评论Id</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpDelete("article/comments/{commentId:int}")]
    [Authorize(Roles = "administrator, evaluator, normaluser")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCommentAsync([FromRoute] int commentId)
    {
        throw new NotImplementedException();
    }
}