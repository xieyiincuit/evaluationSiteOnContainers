namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Controllers;

[Route("api/v1/evaluation")]
[ApiController]
public class EvaluationArticleController : ControllerBase
{
    private readonly IEvaluationArticle _articleService;
    private readonly IEvaluationCategory _categoryService;

    public EvaluationArticleController(IEvaluationArticle articleService, IEvaluationCategory categoryService)
    {
        _articleService = articleService;
        _categoryService = categoryService;
    }

    // GET api/v1/evaluation/articles[?pageSize=10&pageIndex=1]
    [HttpGet]
    [Route("articles")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<EvaluationArticle>), (int)HttpStatusCode.OK)]
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

            return Ok(articles);
        }

        //validate parameter
        var totalArticles = await _articleService.CountArticlesAsync();
        pageSize = pageSize <= 0 || pageSize >= 20 ? 10 : pageSize;

        //计算最大分页数量保证参数合法
        var maxPageIndex = (int)Math.Ceiling((totalArticles / (double)pageSize));
        pageIndex = pageIndex <= 0 || pageIndex > maxPageIndex ? 1 : pageIndex;

        var model = new PaginatedItemsViewModel<EvaluationArticle>(pageIndex, pageSize, totalArticles,
            await _articleService.GetArticlesAsync(pageSize, pageIndex));

        return Ok(model);
    }

    // GET api/v1/evaluation/articles/1
    [HttpGet]
    [Route("articles/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(EvaluationArticle), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<EvaluationArticle>> GetArticleByIdAsync(int id)
    {
        if (id <= 0) return BadRequest();

        var article = await _articleService.GetArticleAsync(id);

        if (article == null) return NotFound();

        return Ok(article);
    }

    // GET api/v1/evaluation/type/articles
    [HttpGet]
    [Route("type/articles")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<EvaluationArticle>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetArticleByTypeAsync(int categoryId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 1)
    {
        var categories = await _categoryService.GetEvaluationCategoriesAsync();

        //invalid categoryId
        if (!categories.Select(x => x.Id).Contains(categoryId))
        {
            return BadRequest("categoryId value invalid, wrong format or no exist");
        }

        //validate parameter
        var totalArticles = await _articleService.CountArticlesByTypeAsync(categoryId);
        pageSize = pageSize <= 0 || pageSize >= 20 ? 10 : pageSize;

        var maxPageIndex = (int)Math.Ceiling((totalArticles / (double)pageSize));
        pageIndex = pageIndex <= 0 || pageIndex > maxPageIndex ? 1 : pageIndex;

        var model = new PaginatedItemsViewModel<EvaluationArticle>(pageIndex, pageSize, totalArticles,
            await _articleService.GetArticlesAsync(pageSize, pageIndex, categoryId));

        return Ok(model);
    }
}