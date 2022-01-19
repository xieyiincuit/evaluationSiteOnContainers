namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Controllers;

[ApiController]
[Route("api/v1/evaluation")]
public class EvaluationCategoryController : ControllerBase
{
    private readonly IEvaluationCategory _categoryService;
    private readonly IMapper _mapper;
    private readonly ILogger<EvaluationCategoryController> _logger;

    public EvaluationCategoryController(IEvaluationCategory categoryService, IMapper mapper, ILogger<EvaluationCategoryController> logger)
    {
        _categoryService = categoryService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    [Route("categories")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(List<EvaluationCategory>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetEvaluateCategoriesAsync()
    {
        var categories = await _categoryService.GetEvaluationCategoriesAsync();
        if (categories == null || categories.Count == 0) return NotFound();
        return Ok(categories);
    }

    [HttpGet]
    [Route("category/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(EvaluationCategory), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetEvaluateCategoryAsync([FromRoute] int id)
    {
        var category = await _categoryService.GetEvaluationCategoryAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPut]
    [Route("category")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> ChangeEvaluateCategoryAsync([FromBody] CategoryUpdateDto categoryUpdateDto)
    {
        if (categoryUpdateDto == null) return BadRequest();
        var entity = await _categoryService.GetEvaluationCategoryAsync(categoryUpdateDto.CategoryId);
        if (entity == null) return NotFound();

        _mapper.Map(categoryUpdateDto, entity);
        await _categoryService.UpdateEvaluationCategoryAsync(entity);
        return NoContent();
    }

    [HttpPost]
    [Route("category")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(EvaluationCategory), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> AddEvaluateCategoryAsync([FromBody] CategoryAddDto categoryAddDto)
    {
        if (categoryAddDto == null) return BadRequest();

        var entity = _mapper.Map<EvaluationCategory>(categoryAddDto);
        await _categoryService.AddEvaluationCategoryAsync(entity);
        return new ObjectResult(entity) { StatusCode = (int)HttpStatusCode.Created };
    }

    [HttpDelete]
    [Route("category/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> RemoveEvaluateCategoryAsync(int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        return await _categoryService.DeleteEvaluationCategoryAsync(id) ? NoContent() : NotFound();
    }
}
