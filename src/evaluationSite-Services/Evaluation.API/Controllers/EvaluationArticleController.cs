namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Controllers;

[Route("api/v1/evaluation")]
[ApiController]
public class EvaluationArticleController : ControllerBase
{
    private readonly IEvaluationArticle _articleService;
    private readonly IEvaluationCategory _categoryService;
    private readonly IMapper _mapper;
    const int _pageSize = 10;

    private readonly Dictionary<int, string> _userDic = new Dictionary<int, string>()
    {
        {1,"Zhousl" },
        {2,"Hanby" },
        {3,"Chenxy" },
        {4,"Wangxb" },
        {5,"Lvcf" },
    };

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
    [ProducesResponseType(typeof(IEnumerable<ArticleDto>), (int)HttpStatusCode.OK)]
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
        if (ParameterValidateHelper.IsInvalidPageIndex(totalArticles, _pageSize, pageIndex)) pageIndex = 1; // pageIndex不合法重设

        var articlesToReturn = _mapper.Map<List<ArticleDto>>(await _articleService.GetArticlesAsync(_pageSize, pageIndex));
        articlesToReturn.ForEach(article => article.Author = _userDic[article.UserId]);

        var model = new PaginatedItemsDtoModel<ArticleDto>(pageIndex, _pageSize, totalArticles, articlesToReturn);
        return Ok(model);
    }

    // GET api/v1/evaluation/articles/1
    [HttpGet]
    [Route("article/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ArticleDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ArticleDto>> GetArticleByIdAsync(int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();

        var article = await _articleService.GetArticleAsync(id);

        if (article == null) return NotFound();

        var articleToReturn = _mapper.Map<ArticleDto>(article);
        articleToReturn.Author = _userDic[article.UserId];

        return Ok(articleToReturn);
    }

    // GET api/v1/evaluation/type/articles
    [HttpGet]
    [Route("type/articles")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<ArticleDto>), (int)HttpStatusCode.OK)]
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

        var articlesToReturn = _mapper.Map<List<ArticleDto>>(await _articleService.GetArticlesAsync(_pageSize, pageIndex, categoryId));
        articlesToReturn.ForEach(article => article.Author = _userDic[article.UserId]);

        var model = new PaginatedItemsDtoModel<ArticleDto>(pageIndex, _pageSize, totalArticles, articlesToReturn);
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

        await _articleService.AddArticleAsync(entity);
        //TODO 用CreateAtXXX替换POST和PUT
        return new ObjectResult(entity) { StatusCode = (int)HttpStatusCode.Created };
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