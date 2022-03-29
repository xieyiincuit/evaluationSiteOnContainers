namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/game")]
public class GameCategoryController : ControllerBase
{
    private const int _pageSize = 10;
    private readonly IGameCategoryService _categoryService;
    private readonly ILogger<GameCategoryController> _logger;
    private readonly IMapper _mapper;

    public GameCategoryController(
        IGameCategoryService categoryService,
        IMapper mapper,
        ILogger<GameCategoryController> logger)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Route("categories/all")]
    public async Task<IActionResult> GetAllCategoryAsync()
    {
        var categores = await _categoryService.GetAllGameCategoriesAsync();
        if (!categores.Any()) return NotFound();
        return Ok(categores);
    }

    [HttpGet]
    [Route("categories")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<GameCategory>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategoriesAsync([FromQuery] int pageIndex = 1)
    {
        var totalCategories = await _categoryService.CountCategoryAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalCategories, _pageSize, pageIndex)) pageIndex = 1;

        var categories = await _categoryService.GetGameCategoriesAsync(pageIndex, _pageSize);
        if (!categories.Any()) return NotFound();

        var model = new PaginatedItemsDtoModel<GameCategory>(pageIndex, _pageSize, totalCategories, categories);
        return Ok(model);
    }

    [HttpGet("category/{categoryId:int}", Name = nameof(GetCategoryAsync))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GameCategory), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategoryAsync([FromRoute] int categoryId)
    {
        if (categoryId <= 0 || categoryId >= int.MaxValue) return BadRequest();

        var category = await _categoryService.GetGameCategoryAsync(categoryId);

        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "administrator")]
    [Route("category")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] GameCategoryAddDto categoryAddDto)
    {
        if (categoryAddDto == null) return BadRequest();

        var entityToAdd = _mapper.Map<GameCategory>(categoryAddDto);
        await _categoryService.AddCategoryAsync(entityToAdd);

        _logger.LogInformation(
            $"administrator: id:{User.FindFirst("sub").Value}, name:{User.Identity.Name} add a category -> categoryName:{categoryAddDto.CategoryName}");

        return CreatedAtRoute(nameof(GetCategoryAsync), new { categoryId = entityToAdd.Id }, null);
    }

    [HttpDelete]
    [Authorize(Roles = "administrator")]
    [Route("category/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        _logger.LogInformation(
            $"administrator: id:{User.FindFirst("sub").Value}, name:{User.Identity.Name} delete a category -> categoryId:{id}");
        var response = await _categoryService.DeleteCategoryAsync(id);
        return response == true ? NoContent() : NotFound();
    }

    [HttpPut]
    [Authorize(Roles = "administrator")]
    [Route("category")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateCategoryAsync([FromBody] GameCategoryUpdateDto categoryUpdateDto)
    {
        if (categoryUpdateDto == null) return BadRequest();
        var entityToUpdate = await _categoryService.GetGameCategoryAsync(categoryUpdateDto.Id);

        if (entityToUpdate == null) return NotFound();
        _logger.LogInformation(
            $"administrator: id:{User.FindFirst("sub").Value}, name:{User.Identity.Name} update a category -> old:{entityToUpdate.CategoryName}, new:{categoryUpdateDto.CategoryName}");

        _mapper.Map(categoryUpdateDto, entityToUpdate);
        await _categoryService.UpdateCategoryAsync(entityToUpdate);

        return NoContent();
    }
}