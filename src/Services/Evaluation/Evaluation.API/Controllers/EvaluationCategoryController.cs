namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Controllers;

[ApiController]
[Route("api/v1/evaluation")]
[Authorize(Roles = _adminRole)]
public class EvaluationCategoryController : ControllerBase
{
    private const string _adminRole = "administrator";
    private readonly IEvaluationCategoryService _categoryService;
    private readonly ILogger<EvaluationCategoryController> _logger;
    private readonly IMapper _mapper;

    public EvaluationCategoryController(
        IEvaluationCategoryService categoryService,
        IMapper mapper,
        ILogger<EvaluationCategoryController> logger)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [AllowAnonymous]
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

    [AllowAnonymous]
    [HttpGet("category/{id:int}", Name = nameof(GetEvaluateCategoryAsync))]
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

        var userId = User.FindFirst("sub").Value;
        _logger.LogInformation("---- administrator:id:{UserId}, name:{Name} update a category -> old:{@old} new:{@new}",
            userId, User.Identity.Name, entity, categoryUpdateDto);
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
        var userId = User.FindFirst("sub").Value;
        _logger.LogInformation("---- administrator:id:{UserId}, name:{Name} add a category -> old:{@old} new:{@new}",
            userId, User.Identity.Name, entity, categoryAddDto);
        return CreatedAtRoute(nameof(GetEvaluateCategoryAsync), new { id = entity.CategoryId }, null);
    }

    [HttpDelete]
    [Route("category/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> RemoveEvaluateCategoryAsync(int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        var userId = User.FindFirst("sub").Value;
        _logger.LogInformation("---- administrator:id:{UserId}, name:{Name} delete a category -> id:{id}",
            userId, User.Identity.Name, id);
        return await _categoryService.DeleteEvaluationCategoryAsync(id) ? NoContent() : NotFound();
    }
}