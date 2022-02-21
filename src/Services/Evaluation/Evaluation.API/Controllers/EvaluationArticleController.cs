namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Controllers;

[Route("api/v1/evaluation")]
[ApiController]
public class EvaluationArticleController : ControllerBase
{
    private const string _adminRole = "administrator";
    private const string _evaluatorRole = "evaluator";
    private const int _pageSize = 10;
    private readonly IEvaluationArticleService _articleService;
    private readonly IEvaluationCategoryService _categoryService;
    private readonly IMapper _mapper;
    private readonly ILogger<EvaluationArticleController> _logger;

    public EvaluationArticleController(
        IEvaluationArticleService articleService,
        IEvaluationCategoryService categoryService,
        IMapper mapper,
        ILogger<EvaluationArticleController> logger)
    {
        _articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // GET api/v1/evaluation/articles[?pageSize=10&pageIndex=1]
    [AllowAnonymous]
    [HttpGet]
    [Route("articles")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<EvaluationArticle>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IEnumerable<ArticleSmallDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetArticlesAsync([FromQuery] int pageIndex = 1, string? ids = null)
    {
        if (!string.IsNullOrEmpty(ids))
        {
            var articles = await _articleService.GetArticlesAsync(_pageSize, pageIndex, ids);

            if (!articles.Any())
                return BadRequest("ids value invalid. Must be comma-separated list of numbers: like ids=1,2,3");

            var articleToReturn = _mapper.Map<List<ArticleDto>>(articles);
            return Ok(articleToReturn);
        }

        //validate pageIndex        
        var totalArticles = await _articleService.CountArticlesAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalArticles, _pageSize, pageIndex))
            pageIndex = 1; // pageIndex不合法重设

        var articlesToReturn = _mapper.Map<List<ArticleSmallDto>>(await _articleService.GetArticlesAsync(_pageSize, pageIndex));

        var model = new PaginatedItemsDtoModel<ArticleSmallDto>(pageIndex, _pageSize, totalArticles, articlesToReturn, null);
        return Ok(model);
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
        
        return Ok(articleToReturn);
    }

    // GET api/v1/evaluation/type/articles
    [AllowAnonymous]
    [HttpGet]
    [Route("type/articles")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<ArticleSmallDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetArticleByTypeAsync(int categoryId, [FromQuery] int pageIndex = 1)
    {
        var categories = await _categoryService.GetEvaluationCategoriesAsync();

        //invalid categoryId
        if (!categories.Select(x => x.CategoryId).Contains(categoryId))
            return BadRequest("categoryId value invalid, wrong format or no exist");

        //validate parameter
        var totalArticles = await _articleService.CountArticlesByTypeAsync(categoryId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalArticles, _pageSize, pageIndex)) pageIndex = 1;

        var articlesToReturn = 
            _mapper.Map<List<ArticleSmallDto>>(await _articleService.GetArticlesAsync(_pageSize, pageIndex, categoryId));

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

        //mapping        
        var entity = _mapper.Map<EvaluationArticle>(articleAddDto);
        entity.UserId = User.FindFirstValue("sub");
        entity.NickName = User.FindFirstValue("nickname");

        await _articleService.AddArticleAsync(entity);
        _logger.LogInformation($"---- evaluator:id:{entity.UserId}, name:{User.Identity.Name} create a article -> id:{entity.ArticleId}, title:{articleAddDto.Title}");
        return CreatedAtRoute(nameof(GetArticleByIdAsync), new { id = entity.ArticleId }, null);
    }

    // Delete api/v1/evaluation/articles/{id}
    [Authorize(Roles = _adminRole)]
    [HttpDelete]
    [Route("article/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteArticleByIdAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();

        if (await _articleService.IsArticleExist(id) == false) return NotFound();

        await _articleService.DeleteArticleAsync(id);

        var userId = User.FindFirst("sub").Value;
        _logger.LogInformation($"---- administrator:id:{userId}, name:{User.Identity.Name} delete a article -> id:{id}");
        return NoContent();
    }

    // Put api/v1/evaluation/articles
    [Authorize(Roles = _evaluatorRole)]
    [HttpPut]
    [Route("article/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateArticleAsync([FromRoute] int id,
        [FromBody] ArticleUpdateDto articleUpdateDto)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        if (await _articleService.IsArticleExist(id) == false) return NotFound();

        var articleToUpdate = await _articleService.GetArticleAsync(id);
        //校验该文章是否为当前请求修改用户吻合
        var userId = User.FindFirst("sub").Value;
        if (userId != articleToUpdate.UserId) return BadRequest();

        _mapper.Map(articleUpdateDto, articleToUpdate);

        await _articleService.UpdateArticleAsync(articleToUpdate);
        _logger.LogInformation("---- evaluator:id:{UserId}, name:{Name} update a article -> id:{Id} content:{@content}",
            userId, User.Identity.Name, id, articleToUpdate);
        return NoContent();
    }
}