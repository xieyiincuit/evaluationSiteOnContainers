namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Controllers;

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
    private readonly IMapper _mapper;

    public EvaluationArticleController(
        IEvaluationArticleService articleService,
        IEvaluationCategoryService categoryService,
        IEvaluationCommentService commentService,
        IMapper mapper,
        ILogger<EvaluationArticleController> logger,
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
    }

    // GET api/v1/evaluation/articles[?pageSize=10&pageIndex=1]
    [AllowAnonymous]
    [HttpGet]
    [Route("articles")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<ArticleSmallDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetArticlesAsync([FromQuery] int pageIndex = 1)
    {
        //validate pageIndex        
        var totalArticles = await _articleService.CountArticlesAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalArticles, _pageSize, pageIndex))
            pageIndex = 1; // pageIndex不合法重设

        var articlesToReturn = await _articleService.GetArticlesAsync(_pageSize, pageIndex);

        //获取评论数量
        foreach (var smallDto in articlesToReturn)
        {
            smallDto.CommentsCount = await _commentService.CountArticleRootCommentsAsync(smallDto.ArticleId);
        }

        var model = new PaginatedItemsDtoModel<ArticleSmallDto>(pageIndex, _pageSize, totalArticles, articlesToReturn, null);
        return Ok(model);
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("game/articles")]
    public async Task<IActionResult> GetArticlesByGameAsync([FromQuery] int gameId, [FromQuery] int pageIndex = 1)
    {
        //validate pageIndex        
        var totalArticles = await _articleService.CountArticlesByGameAsync(gameId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalArticles, _pageSize, pageIndex))
            pageIndex = 1; // pageIndex不合法重设

        var articlesToReturn = await _articleService.GetArticlesByGameAsync(_pageSize, pageIndex, gameId);

        //获取评论数量和UserId
        var userIds = new HashSet<string>();
        foreach (var smallDto in articlesToReturn)
        {
            smallDto.CommentsCount = await _commentService.CountArticleRootCommentsAsync(smallDto.ArticleId);
            userIds.Add(smallDto.AuthorId);
        }
        //调用链的最上方释放
        using var response = await _identityService.GetCommentsUserProfileAsync(userIds.ToList());
        var userInfoDto = new List<UserAvatarDto>();
        if (response.IsSuccessStatusCode) userInfoDto = await response.Content.ReadFromJsonAsync<List<UserAvatarDto>>();

        var model = new PaginatedItemsDtoModel<ArticleGameDto>(pageIndex, _pageSize, totalArticles, articlesToReturn, userInfoDto);
        return Ok(model);
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("shop/articles")]
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

    // GET api/v1/evaluation/articles/1
    [AllowAnonymous]
    [HttpGet("article/{id:int}", Name = nameof(GetArticleByIdAsync))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ArticleDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ArticleDto>> GetArticleByIdAsync(int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();

        var article = await _articleService.GetArticleAsync(id);

        if (article == null) return NotFound();

        var articleToReturn = _mapper.Map<ArticleDto>(article);
        articleToReturn.CommentsCount = await _commentService.CountArticleRootCommentsAsync(articleToReturn.ArticleId);

        return Ok(articleToReturn);
    }

    // GET api/v1/evaluation/type/articles
    [AllowAnonymous]
    [HttpGet]
    [Route("articles/type")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<ArticleSmallDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetArticleByTypeAsync([FromQuery] int categoryId, [FromQuery] int pageIndex = 1)
    {
        var categories = await _categoryService.GetEvaluationCategoriesAsync();

        //invalid categoryId
        if (!categories.Select(x => x.CategoryId).Contains(categoryId))
            return BadRequest("categoryId value invalid, wrong format or no exist");

        //validate parameter
        var totalArticles = await _articleService.CountArticlesByTypeAsync(categoryId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalArticles, _pageSize, pageIndex)) pageIndex = 1;

        var articlesToReturn = await _articleService.GetArticlesAsync(_pageSize, pageIndex, categoryId);

        //获取评论数量
        foreach (var article in articlesToReturn)
        {
            article.CommentsCount = await _commentService.CountArticleRootCommentsAsync(article.ArticleId);
        }

        var model = new PaginatedItemsDtoModel<ArticleSmallDto>(pageIndex, _pageSize, totalArticles, articlesToReturn, null);
        return Ok(model);
    }

    // Post api/v1/evaluation/articles
    [Authorize(Roles = _evaluatorRole)]
    [HttpPost]
    [Route("article")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(EvaluationArticle), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateArticleAsync([FromBody] ArticleAddDto articleAddDto)
    {
        if (articleAddDto == null) return BadRequest();

        if (await _gameRepoGrpcClient.CheckGameExistAsync(articleAddDto.GameId) == false) return BadRequest();

        //mapping        
        var entity = _mapper.Map<EvaluationArticle>(articleAddDto);
        //grpc通信获取游戏信息
        var gameInfo = await _gameRepoGrpcClient.GetGameInfoAsync(articleAddDto.GameId);
        entity.GameName = gameInfo.GameName;
        entity.DescriptionImage = gameInfo.DescriptionPic;

        entity.UserId = User.FindFirstValue("sub");
        entity.NickName = User.FindFirstValue("nickname");

        await _articleService.AddArticleAsync(entity);
        _logger.LogInformation($"---- evaluator:id:{entity.UserId}, name:{User.Identity.Name} create a article -> id:{entity.ArticleId}, title:{articleAddDto.Title}");
        return CreatedAtRoute(nameof(GetArticleByIdAsync), new { id = entity.ArticleId }, new { articleId = entity.ArticleId });

    }

    [Authorize(Roles = _evaluatorRole)]
    [HttpGet]
    [Route("my/articles")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserArticlesAsync([FromQuery] int pageIndex,
        [FromQuery] int? categoryId, [FromQuery] bool timeDesc = true)
    {
        var currentUserId = User.FindFirst("sub").Value;
        int pageSize = 15;
        var totalArticles = await _articleService.CountArticlesByUserAsync(currentUserId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalArticles, pageSize, pageIndex))
            pageIndex = 1; // pageIndex不合法重设

        var userArticles = await _articleService.GetUserArticlesAsync(pageSize, pageIndex, currentUserId, categoryId, timeDesc);

        var model = new PaginatedItemsDtoModel<ArticleTableDto>(pageIndex, pageSize, totalArticles, userArticles, null);
        return Ok(model);
    }

    /// <summary>
    /// 管理员获取所有测评文章列表
    /// </summary>
    /// <param name="pageIndex">pageSize为15</param>
    /// <param name="categoryId">可分类型筛选</param>
    /// <returns></returns>
    //[Authorize(Roles = _adminRole)]
    [HttpGet]
    [Route("admin/articles")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetArticlesForAdminAsync([FromQuery] int pageIndex, [FromQuery] int? categoryId)
    {
        int pageSize = 15;
        int totalArticles = categoryId.HasValue
            ? await _articleService.CountArticlesByTypeAsync(categoryId.Value)
            : await _articleService.CountArticlesAsync();

        if (ParameterValidateHelper.IsInvalidPageIndex(totalArticles, pageSize, pageIndex))
            pageIndex = 1; // pageIndex不合法重设

        List<ArticleSmallDto>? articles = categoryId.HasValue
            ? await _articleService.GetArticlesAsync(pageSize, pageIndex, categoryId.Value)
            : await _articleService.GetArticlesAsync(pageSize, pageIndex);

        var model = new PaginatedItemsDtoModel<ArticleSmallDto>(pageIndex, pageSize, totalArticles, articles, null);
        return Ok(model);
    }

    // Delete api/v1/evaluation/articles/{id}
    [Authorize(Roles = $"{_adminRole}, {_evaluatorRole}")]
    [HttpDelete]
    [Route("article/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteArticleByIdAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        if (await _articleService.IsArticleExist(id) == false) return NotFound();

        var userId = User.FindFirst("sub").Value;
        if (User.FindFirstValue("role") == _evaluatorRole)
        {
            var article = await _articleService.GetArticleAsync(id);
            if (article.UserId != userId) return BadRequest();
        }

        await _articleService.DeleteArticleAsync(id);
        _logger.LogInformation($"---- administrator:id:{userId}, name:{User.Identity.Name} delete a article -> id:{id}");
        return NoContent();
    }

    // Put api/v1/evaluation/articles
    [Authorize(Roles = _evaluatorRole)]
    [HttpPut]
    [Route("article")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateArticleAsync([FromBody] ArticleUpdateDto articleUpdateDto)
    {
        if (articleUpdateDto.Id <= 0 || articleUpdateDto.Id >= int.MaxValue) return BadRequest();
        if (await _articleService.IsArticleExist(articleUpdateDto.Id) == false) return NotFound();

        var articleToUpdate = await _articleService.GetArticleAsync(articleUpdateDto.Id);
        //校验该文章是否为当前请求修改用户吻合
        var userId = User.FindFirst("sub").Value;
        if (userId != articleToUpdate.UserId) return BadRequest();

        _mapper.Map(articleUpdateDto, articleToUpdate);
        await _articleService.UpdateArticleAsync(articleToUpdate);
        _logger.LogInformation("---- evaluator:id:{UserId}, name:{Name} update a article -> id:{Id} content:{@content}",
            userId, User.Identity.Name, articleUpdateDto.Id, articleToUpdate);
        return NoContent();
    }
}