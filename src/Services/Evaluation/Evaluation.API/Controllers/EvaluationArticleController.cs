namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Controllers;

/// <summary>
/// 游戏测评文章接口
/// </summary>
[Route("api/v1")]
[ApiController]
public class EvaluationArticleController : ControllerBase
{
    private const string _adminRole = "administrator";
    private const string _evaluatorRole = "evaluator";
    private const int _pageSize = 5;
    private readonly IEvaluationArticleService _articleService;
    private readonly IEvaluationCategoryService _categoryService;
    private readonly IEvaluationCommentService _commentService;
    private readonly IdentityCallService _identityService;
    private readonly GameRepoGrpcService _gameRepoGrpcClient;
    private readonly ILogger<EvaluationArticleController> _logger;
    private readonly IRedisDatabase _redisDatabase;
    private readonly IMapper _mapper;

    public EvaluationArticleController(
        IEvaluationArticleService articleService,
        IEvaluationCategoryService categoryService,
        IEvaluationCommentService commentService,
        IMapper mapper,
        ILogger<EvaluationArticleController> logger,
        IRedisDatabase redisDatabase,
        IdentityCallService identityService,
        GameRepoGrpcService gameRepoGrpcClient)
    {
        _articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        _gameRepoGrpcClient = gameRepoGrpcClient ?? throw new ArgumentNullException(nameof(gameRepoGrpcClient));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
    }

    /// <summary>
    /// 用户——分页获取所有游戏测评文章
    /// </summary>
    /// <param name="pageIndex">pageSize=5</param>
    /// <returns></returns>
    [HttpGet("articles")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<ArticleSmallDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetArticlesAsync([FromQuery] int pageIndex = 1)
    {
        var totalArticles = await _articleService.CountArticlesAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalArticles, _pageSize, pageIndex))
            pageIndex = 1; // pageIndex不合法重设

        var articlesToReturn = await _articleService.GetArticlesAsync(_pageSize, pageIndex);

        //获取评论数量
        foreach (var smallDto in articlesToReturn)
        {
            smallDto.CommentsCount = await _commentService.CountArticleRootCommentsAsync(smallDto.ArticleId);
        }

        var model = new PaginatedItemsDtoModel<ArticleSmallDto>(pageIndex, _pageSize, totalArticles, articlesToReturn);
        return Ok(model);
    }

    /// <summary>
    /// 用户——获取某个游戏的测评文章
    /// </summary>
    /// <param name="gameId">游戏Id</param>
    /// <param name="pageIndex">pageSize=5</param>
    /// <returns></returns>
    [HttpGet("game/articles")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<ArticleGameDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetArticlesByGameAsync([FromQuery] int gameId, [FromQuery] int pageIndex = 1)
    {
        var totalArticles = await _articleService.CountArticlesByGameAsync(gameId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalArticles, _pageSize, pageIndex))
            pageIndex = 1; // pageIndex不合法重设

        var articlesToReturn = await _articleService.GetArticlesByGameAsync(_pageSize, pageIndex, gameId);

        // 获取评论数量和UserId
        var userIds = new HashSet<string>();
        foreach (var smallDto in articlesToReturn)
        {
            smallDto.CommentsCount = await _commentService.CountArticleRootCommentsAsync(smallDto.ArticleId);
            userIds.Add(smallDto.AuthorId);
        }

        // 获取用户额外信息
        using var response = await _identityService.GetCommentsUserProfileAsync(userIds.ToList());
        var userInfoDto = new List<UserAvatarDto>();
        if (response.IsSuccessStatusCode) userInfoDto = await response.Content.ReadFromJsonAsync<List<UserAvatarDto>>();

        var model = new PaginatedItemsWithUserDtoModel<ArticleGameDto>(pageIndex, _pageSize, totalArticles, articlesToReturn, userInfoDto);
        return Ok(model);
    }

    /// <summary>
    /// 用户——获取测评文章详细信息
    /// </summary>
    /// <param name="id">文章Id</param>
    /// <returns></returns>
    [HttpGet("article/{id:int}", Name = nameof(GetArticleByIdAsync))]
    [AllowAnonymous]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ArticleDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ArticleDto>> GetArticleByIdAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        var article = await _articleService.GetArticleAsync(id);
        if (article == null) return NotFound();

        var articleToReturn = _mapper.Map<ArticleDto>(article);
        articleToReturn.CommentsCount = await _commentService.CountArticleRootCommentsAsync(articleToReturn.ArticleId);

        var redisKey = $"view_article_{id}";
        if (!await _redisDatabase.Database.KeyExistsAsync(redisKey))
        {
            await _redisDatabase.Database.StringSetAsync(redisKey, article.JoinCount + 1);
        }
        else
        {
            var newJoin = await _redisDatabase.Database.StringIncrementAsync(redisKey);
            articleToReturn.JoinCount = (int)newJoin;
        }
        return Ok(articleToReturn);
    }

    /// <summary>
    /// 用户——根据测评类别获取特定测评文章
    /// </summary>
    /// <param name="categoryId">类别Id</param>
    /// <param name="pageIndex">pageSize=5</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("articles/type")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<ArticleSmallDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetArticleByTypeAsync([FromQuery] int categoryId, [FromQuery] int pageIndex = 1)
    {
        var categories = await _categoryService.GetEvaluationCategoriesAsync();

        // 非法的类型Id
        if (!categories.Select(x => x.CategoryId).Contains(categoryId))
            return BadRequest("categoryId value invalid, wrong format or no exist");

        var totalArticles = await _articleService.CountArticlesByTypeAsync(categoryId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalArticles, _pageSize, pageIndex)) pageIndex = 1;

        var articlesToReturn = await _articleService.GetArticlesAsync(_pageSize, pageIndex, categoryId);

        //获取评论数量
        foreach (var article in articlesToReturn)
        {
            article.CommentsCount = await _commentService.CountArticleRootCommentsAsync(article.ArticleId);
        }

        var model = new PaginatedItemsDtoModel<ArticleSmallDto>(pageIndex, _pageSize, totalArticles, articlesToReturn);
        return Ok(model);
    }

    /// <summary>
    /// 测评人员——新增游戏测评文章
    /// </summary>
    /// <param name="articleAddDto"></param>
    /// <returns></returns>
    [Authorize(Roles = _evaluatorRole)] //表示该接口需要身份认证并且只授权给身份为“evaluator”的用户
    [HttpPost("article")] //请求路由
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(EvaluationArticle), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateArticleAsync([FromBody] ArticleAddDto articleAddDto)
    {
        //若HttpRequest body为空，返回BadRequest
        if (articleAddDto == null) return BadRequest();
        //若该测评文章关联的游戏不存在，返回BadRequest
        if (await _gameRepoGrpcClient.CheckGameExistAsync(articleAddDto.GameId) == false) return BadRequest();
        //mapping实体        
        var entity = _mapper.Map<EvaluationArticle>(articleAddDto);
        //grpc通信游戏资料服务获取游戏信息
        var gameInfo = await _gameRepoGrpcClient.GetGameInfoAsync(articleAddDto.GameId);
        //将游戏姓名关联记录到测评文章中
        entity.GameName = gameInfo.GameName;
        //将游戏描述图片关联记录到测评文章中
        entity.DescriptionImage = gameInfo.DescriptionPic;
        //记录测评人员Id
        entity.UserId = User.FindFirstValue("sub");
        //记录测评人员姓名
        entity.NickName = User.FindFirstValue("nickname");
        //提交新增测评文章事务
        var addResponse = await _articleService.AddArticleAsync(entity);
        if (addResponse != false) throw new EvaluationDomainException("创建测评文章失败");
        //记录日志并返回结果
        _logger.LogInformation($"---- evaluator:id:{entity.UserId}, name:{User.FindFirst("nickname")} create a article -> id:{entity.ArticleId}, title:{articleAddDto.Title}");
        //成功返回Created 201结果
        return CreatedAtRoute(nameof(GetArticleByIdAsync), new { id = entity.ArticleId }, new { articleId = entity.ArticleId });
    }

    /// <summary>
    /// 测评人员——获取自己的测评文章
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="categoryId"></param>
    /// <param name="timeDesc"></param>
    /// <returns></returns>
    [Authorize(Roles = _evaluatorRole)]
    [HttpGet("my/articles")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserArticlesAsync([FromQuery] int pageIndex, [FromQuery] int? categoryId, [FromQuery] bool timeDesc = true)
    {
        // 用户文章列表分页数
        int pageSize = 15;

        var currentUserId = User.FindFirst("sub").Value;
        var totalArticles = await _articleService.CountArticlesByUserAsync(currentUserId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalArticles, pageSize, pageIndex))
            pageIndex = 1;

        // 获取当前测评人员的文章
        var userArticles = await _articleService.GetUserArticlesAsync(pageSize, pageIndex, currentUserId, categoryId, timeDesc);

        var model = new PaginatedItemsDtoModel<ArticleTableDto>(pageIndex, pageSize, totalArticles, userArticles);
        return Ok(model);
    }

    /// <summary>
    /// 管理员——获取所有测评文章列表
    /// </summary>
    /// <param name="pageIndex">pageSize为15</param>
    /// <param name="categoryId">可分类型筛选</param>
    /// <returns></returns>
    [Authorize(Roles = _adminRole)]
    [HttpGet("admin/articles")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<ArticleSmallDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetArticlesForAdminAsync([FromQuery] int pageIndex, [FromQuery] int? categoryId)
    {
        int pageSize = 15;
        int totalArticles = categoryId.HasValue
            ? await _articleService.CountArticlesByTypeAsync(categoryId.Value)
            : await _articleService.CountArticlesAsync();

        if (ParameterValidateHelper.IsInvalidPageIndex(totalArticles, pageSize, pageIndex))
            pageIndex = 1; // pageIndex不合法重设

        List<ArticleSmallDto> articles = categoryId.HasValue
            ? await _articleService.GetArticlesAsync(pageSize, pageIndex, categoryId.Value)
            : await _articleService.GetArticlesAsync(pageSize, pageIndex);

        //获取评论数量
        foreach (var article in articles)
        {
            article.CommentsCount = await _commentService.CountArticleRootCommentsAsync(article.ArticleId);
        }

        var model = new PaginatedItemsDtoModel<ArticleSmallDto>(pageIndex, pageSize, totalArticles, articles);
        return Ok(model);
    }


    /// <summary>
    /// 测评人员，管理员——删除测评文章
    /// </summary>
    /// <param name="id">文章Id</param>
    /// <returns></returns>
    [Authorize(Roles = $"{_adminRole}, {_evaluatorRole}")]
    [HttpDelete("article/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteArticleByIdAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        if (await _articleService.IsArticleExist(id) == false) return BadRequest();

        var userId = User.FindFirst("sub").Value;
        if (User.FindFirstValue("role") == _evaluatorRole)
        {
            var article = await _articleService.GetArticleAsync(id);
            if (article.UserId != userId) return BadRequest();
        }

        var delResponse = await _articleService.DeleteArticleAsync(id);
        if (delResponse == false)
        {
            _logger.LogError($"---- user:id:{userId}, name:{User.FindFirst("nickname")} delete a article error-> id:{id}");
            throw new EvaluationDomainException("文章删除失败");
        }

        _logger.LogInformation($"---- administrator:id:{userId}, name:{User.FindFirst("nickname")} delete a article -> id:{id}");
        return NoContent();
    }

    /// <summary>
    /// 测评人员——修改自己的测评文章内容
    /// </summary>
    /// <param name="articleUpdateDto"></param>
    /// <returns></returns>
    [Authorize(Roles = _evaluatorRole)]
    [HttpPut("article")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateArticleAsync([FromBody] ArticleUpdateDto articleUpdateDto)
    {
        if (articleUpdateDto.Id <= 0 || articleUpdateDto.Id >= int.MaxValue) return BadRequest();
        if (await _articleService.IsArticleExist(articleUpdateDto.Id) == false) return BadRequest();

        var articleToUpdate = await _articleService.GetArticleAsync(articleUpdateDto.Id);

        //校验该文章是否为当前请求修改用户吻合
        var userId = User.FindFirst("sub").Value;
        if (userId != articleToUpdate.UserId) return BadRequest();

        //更新文章内容
        _mapper.Map(articleUpdateDto, articleToUpdate);
        var updateResponse = await _articleService.UpdateArticleAsync(articleToUpdate);
        if (updateResponse == false)
        {
            _logger.LogError("---- evaluator:id:{UserId}, name:{Name} update a article error -> id:{Id} content:{@content}",
                userId, User.FindFirst("nickname"), articleUpdateDto.Id, articleToUpdate);
        }

        _logger.LogInformation("---- evaluator:id:{UserId}, name:{Name} update a article -> id:{Id} content:{@content}",
            userId, User.FindFirst("nickname"), articleUpdateDto.Id, articleToUpdate);
        return NoContent();
    }

    /// <summary>
    /// 用户——点赞测评文章
    /// </summary>
    /// <param name="likeAddDto"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("like")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> LikeArticleAsync([FromBody] ArticleLikeAddDto likeAddDto)
    {
        var currentUser = User.FindFirstValue("sub");
        var likeResponse = await _articleService.LikeArticleAsync(likeAddDto.ArticleId, currentUser);
        return likeResponse == 1 ? Ok("点赞成功^ ^") : Ok("请勿重复点赞^ ^");
    }

    /// <summary>
    /// 供GameRepo服务调用的接口
    /// </summary>
    /// <param name="gameIds"></param>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("shop/articles")]
    [AllowAnonymous]
    public async Task<IActionResult> GetArticlesByShopItemAsync([FromBody] List<int> gameIds)
    {
        if (gameIds == null || gameIds.Count == 0) return BadRequest();

        var result = new List<ArticleShopDto>();
        foreach (var gameId in gameIds)
        {
            var article = await _articleService.GetArticlesByShopItemAsync(gameId);
            if (article == null) continue;
            else result.Add(article);
        }
        return Ok(result);
    }
}