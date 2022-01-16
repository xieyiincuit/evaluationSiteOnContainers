namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Controllers;

[Route("api/v1/evaluation")]
[ApiController]
public class EvaluationArticleController : ControllerBase
{
    private readonly IEvaluationArticle _articleService;
    private readonly IEvaluationCategory _categoryService;
    private readonly IMapper _mapper;

    public EvaluationArticleController(IEvaluationArticle articleService, IEvaluationCategory categoryService,
        IMapper mapper)
    {
        _articleService = articleService;
        _categoryService = categoryService;
        _mapper = mapper;
    }

    // GET api/v1/evaluation/articles[?pageSize=10&pageIndex=1]
    [HttpGet]
    [Route("articles")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<EvaluationArticle>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IEnumerable<EvaluationArticle>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetArticlesAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 1, string? ids = null)
    {
        if (!string.IsNullOrEmpty(ids))
        {
            var articles = await _articleService.GetArticlesAsync(pageSize, pageIndex, ids);
            if (!articles.Any())
            {
                return BadRequest("ids value invalid. Must be comma-separated list of numbers: like ids=1,2,3");
            }

            var articleToReturn = _mapper.Map<List<ArticleDto>>(articles);
            return Ok(articleToReturn);
        }

        //validate parameter
        var totalArticles = await _articleService.CountArticlesAsync();
        pageSize = pageSize <= 0 || pageSize >= 20 ? 10 : pageSize;

        //计算最大分页数量保证参数合法
        var maxPageIndex = (int)Math.Ceiling((totalArticles / (double)pageSize));
        pageIndex = pageIndex <= 0 || pageIndex > maxPageIndex ? 1 : pageIndex;

        var articlesToReturn = _mapper.Map<List<ArticleDto>>(await _articleService.GetArticlesAsync(pageSize, pageIndex));
        var model = new PaginatedItemsDtoModel<ArticleDto>(pageIndex, pageSize, totalArticles, articlesToReturn);

        return Ok(model);
    }

    // GET api/v1/evaluation/articles/1
    [HttpGet]
    [Route("article/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(EvaluationArticle), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ArticleDto>> GetArticleByIdAsync(int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();

        var article = await _articleService.GetArticleAsync(id);

        if (article == null) return NotFound();

        var articleToReturn = _mapper.Map<ArticleDto>(article);
        return Ok(articleToReturn);
    }

    // GET api/v1/evaluation/type/articles
    [HttpGet]
    [Route("type/articles")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<EvaluationArticle>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetArticleByTypeAsync(int categoryId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 1)
    {
        var categories = await _categoryService.GetEvaluationCategoriesAsync();

        //invalid categoryId
        if (!categories.Select(x => x.CategoryId).Contains(categoryId))
        {
            return BadRequest("categoryId value invalid, wrong format or no exist");
        }

        //validate parameter
        var totalArticles = await _articleService.CountArticlesByTypeAsync(categoryId);
        pageSize = pageSize <= 0 || pageSize >= 20 ? 10 : pageSize;

        var maxPageIndex = (int)Math.Ceiling((totalArticles / (double)pageSize));
        pageIndex = pageIndex <= 0 || pageIndex > maxPageIndex ? 1 : pageIndex;

        var articlesToReturn = _mapper.Map<List<ArticleDto>>(await _articleService.GetArticlesAsync(pageSize, pageIndex, categoryId));
        var model = new PaginatedItemsDtoModel<ArticleDto>(pageIndex, pageSize, totalArticles, articlesToReturn);

        return Ok(model);
    }


    // Post api/v1/evaluation/articles
    [HttpPost]
    [Route("article")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(EvaluationArticle), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateArticleAsync([FromBody] ArticleAddDto articleAddDto)
    {
        if (articleAddDto == null) return BadRequest();

        //mapping        
        var entity = _mapper.Map<EvaluationArticle>(articleAddDto);
        //TODO Author从用户信息中获取
        entity.UserId = 1;
        entity.CreateTime = DateTime.Now.ToLocalTime();

        await _articleService.AddArticleAsync(entity);
        return CreatedAtRoute(nameof(GetArticleByIdAsync), new { id = entity.ArticleId }, entity);
    }

    // Delete api/v1/evaluation/articles/{id}
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
        return NoContent();
    }

    // Put api/v1/evaluation/articles
    [HttpPut]
    [Route("article/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateArticleAsync([FromRoute] int id, [FromBody] ArticleUpdateDto articleUpdateDto)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        if (await _articleService.IsArticleExist(id) == false) return NotFound();

        var articleToUpdate = await _articleService.GetArticleAsync(id);
        _mapper.Map(articleUpdateDto, articleToUpdate);

        await _articleService.UpdateArticleAsync(articleToUpdate);
        return NoContent();
    }
}