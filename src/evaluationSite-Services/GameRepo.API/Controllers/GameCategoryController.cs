namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/g")]
public class GameCategoryController : ControllerBase
{
    private readonly IGameCategoryService _categoryService;
    private readonly IMapper _mapper;
    private const int _pageSize = 10;
    public GameCategoryController(IGameCategoryService categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("categories")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<GameCategory>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategoriesAsync([FromQuery] int pageIndex = 1)
    {
        var totalCategoies = await _categoryService.CountCategoryAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalCategoies, _pageSize, pageIndex)) pageIndex = 1;

        var categoies = await _categoryService.GetGameCategoriesAsync(pageIndex, _pageSize);
        if (!categoies.Any()) return NotFound();

        var model = new PaginatedItemsDtoModel<GameCategory>(pageIndex, _pageSize, totalCategoies, categoies);
        return Ok(model);
    }

    [HttpGet("category/{categoryId:int}", Name = nameof(GetCategoryAsync))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GameCategory), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategoryAsync(int categoryId)
    {
        if (categoryId <= 0 || categoryId >= int.MaxValue) return BadRequest();

        var category = await _categoryService.GetGameCategoryAsync(categoryId);

        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    [Route("category")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] GameCategoryAddDto categoryAddDto)
    {
        if (categoryAddDto == null) return BadRequest();

        var entityToadd = _mapper.Map<GameCategory>(categoryAddDto);

        await _categoryService.AddCategoryAsync(entityToadd);
        return CreatedAtRoute(nameof(GetCategoryAsync), new { categoryId = entityToadd.Id }, null);
    }

    [HttpDelete]
    [Route("category/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCategoryAsync(int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();

        var result = await _categoryService.DeleteCategoryAsync(id);
        return result == true ? NoContent() : NotFound();
    }

    [HttpPut]
    [Route("category")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateCategoryAsync([FromBody] GameCategoryUpdateDto categoryUpdateDto)
    {
        if (categoryUpdateDto == null) return BadRequest();

        var entityToUpdate = _mapper.Map<GameCategory>(categoryUpdateDto);
        await _categoryService.UpdeteCategoryAsync(entityToUpdate);
        return NoContent();
    }
}
