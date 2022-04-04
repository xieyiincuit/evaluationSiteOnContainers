namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Controllers;

/// <summary>
/// 管理测评文章分类
/// </summary>
[ApiController]
[Route("api/v1/category")]
[Authorize(Roles = _adminRole)]
public class EvaluationCategoryController : ControllerBase
{
    private const string _adminRole = "administrator";
    private const string _categoryListKey = "categories";
    private readonly IEvaluationCategoryService _categoryService;
    private readonly IRedisDatabase _redisDatabase;
    private readonly ILogger<EvaluationCategoryController> _logger;
    private readonly IMapper _mapper;

    public EvaluationCategoryController(
        IEvaluationCategoryService categoryService,
        IRedisDatabase redisDatabase,
        IMapper mapper,
        ILogger<EvaluationCategoryController> logger)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 获取所有测评分类
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("list")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(List<EvaluationCategory>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetEvaluateCategoriesAsync()
    {
        if (await _redisDatabase.Database.KeyExistsAsync(_categoryListKey))
        {
            var cacheCategory = await _redisDatabase.GetAsync<List<EvaluationCategory>>(_categoryListKey);
            _logger.LogInformation("categories get from redis @{categoryList}", cacheCategory);
            return Ok(cacheCategory);
        }

        var categories = await _categoryService.GetEvaluationCategoriesAsync();
        if (categories == null || categories.Count == 0) return NotFound();
        await _redisDatabase.AddAsync(_categoryListKey, categories);
        return Ok(categories);
    }


    /// <summary>
    /// 根据Id获取特定测评分类
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("{id:int}", Name = nameof(GetEvaluateCategoryAsync))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(EvaluationCategory), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetEvaluateCategoryAsync([FromRoute] int id)
    {
        var category = await _categoryService.GetEvaluationCategoryAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    /// <summary>
    /// 修改分类名称
    /// </summary>
    /// <param name="categoryUpdateDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(Roles = "administrator")]
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

        if (await _redisDatabase.Database.KeyExistsAsync(_categoryListKey))
            await _redisDatabase.Database.KeyDeleteAsync(_categoryListKey);

        return NoContent();
    }

    /// <summary>
    /// 新增分类
    /// </summary>
    /// <param name="categoryAddDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "administrator")]
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
        if (await _redisDatabase.Database.KeyExistsAsync(_categoryListKey))
            await _redisDatabase.Database.KeyDeleteAsync(_categoryListKey);
        return CreatedAtRoute(nameof(GetEvaluateCategoryAsync), new { id = entity.CategoryId }, null);
    }

    /// <summary>
    /// 删除分类（该分类下有文章会被数据库约束阻止）
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> RemoveEvaluateCategoryAsync(int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        var userId = User.FindFirst("sub").Value;
        _logger.LogInformation("---- administrator:id:{UserId}, name:{Name} delete a category -> id:{id}",
            userId, User.Identity.Name, id);
        if (await _redisDatabase.Database.KeyExistsAsync(_categoryListKey))
            await _redisDatabase.Database.KeyDeleteAsync(_categoryListKey);
        return await _categoryService.DeleteEvaluationCategoryAsync(id) ? NoContent() : NotFound();
    }
}