namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Controllers;

[Route("api/v1/evaluation")]
[ApiController]
public class EvaluationCommentController : ControllerBase
{
    private readonly IEvaluationArticle _articleService;
    private readonly IEvaluationComment _commentService;

    public EvaluationCommentController(IEvaluationArticle articleService, IEvaluationComment commentService)
    {
        _articleService = articleService;
        _commentService = commentService;
    }

    [HttpGet]
    [Route("article/{articleId:int}/comments")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<ArticleCommentDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetArticleComments([FromRoute] int articleId, [FromQuery] int pageIndex = 1)
    {
        const int pageSize = 5;
        if (articleId <= 0 || articleId >= int.MaxValue) return BadRequest();

        if (await _articleService.IsArticleExist(articleId) == false) return NotFound();

        var totalComments = await _commentService.CountArticleRootCommentsAsync(articleId);
        var maxPageIndex = (int)Math.Ceiling((totalComments / (double)pageSize));
        if (pageIndex > maxPageIndex) return NotFound();
       
        var comments = await _commentService.GetArticleCommentsAsync(pageIndex, pageSize, articleId);

        var model = new PaginatedItemsDtoModel<ArticleCommentDto>(pageIndex, pageSize, totalComments, comments);
        return Ok(model);
    }

    [HttpGet]
    [Route("user/{userId:int}/comments")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserComments([FromRoute] int userId)
    {
        if (userId <= 0 || userId >= int.MaxValue) return BadRequest();
        //这里的userId 应该从Token中获取
        var comments = await _commentService.GetUserCommentsAsync(userId);
        return Ok(comments);
    }
}
