namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Controllers;

/// <summary>
/// 测评类别接口
/// </summary>
[ApiController]
[Route("api/v1/category")]
public class EvaluationCategoryController : ControllerBase
{
    private const string _adminRole = "administrator";
    private const string _categoryListKey = "evaluationCategories";
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
    /// 用户，管理员——获取所有的测评分类
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("list")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(List<EvaluationCategory>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetEvaluateCategoriesAsync()
    {
        if (await _redisDatabase.Database.KeyExistsAsync(_categoryListKey))
        {
            var cacheCategory = await _redisDatabase.GetAsync<List<EvaluationCategory>>(_categoryListKey);
            _logger.LogInformation("categories get from redis @{CategoryList}", cacheCategory);
            return Ok(cacheCategory);
        }

        var categories = await _categoryService.GetEvaluationCategoriesAsync();
        if (categories == null || categories.Count == 0) return NotFound();

        var redisResponse = await _redisDatabase.AddAsync(_categoryListKey, categories);
        if (redisResponse == false)
        {
            _logger.LogWarning("redis add evaluation category @{CategoryList} Error", categories);
        }
        return Ok(categories);
    }

    /// <summary>
    /// 用户，管理员——根据Id获取特定测评分类
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
        return category == null ? NotFound() : Ok(category);
    }

    /// <summary>
    /// 管理员——修改测评分类名称
    /// </summary>
    /// <param name="categoryUpdateDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(Roles = _adminRole)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> ChangeEvaluateCategoryAsync([FromBody] CategoryUpdateDto categoryUpdateDto)
    {
        if (categoryUpdateDto == null) return BadRequest();

        var entity = await _categoryService.GetEvaluationCategoryAsync(categoryUpdateDto.CategoryId);
        if (entity == null) return BadRequest();

        _mapper.Map(categoryUpdateDto, entity);
        var updateResponse = await _categoryService.UpdateEvaluationCategoryAsync(entity);
        if (updateResponse == false)
        {
            _logger.LogError("---- administrator:id:{UserId}, name:{Name} update a category error -> old:{@old} new:{@new}",
                User.FindFirst("sub").Value, User.Identity.Name, entity, categoryUpdateDto);
            throw new EvaluationDomainException("更新测评名称失败");
        }

        _logger.LogInformation("---- administrator:id:{UserId}, name:{Name} update a category -> old:{@old} new:{@new}",
            User.FindFirst("sub").Value, User.Identity.Name, entity, categoryUpdateDto);

        if (await _redisDatabase.Database.KeyExistsAsync(_categoryListKey))
            await _redisDatabase.Database.KeyDeleteAsync(_categoryListKey);

        return NoContent();
    }

    /// <summary>
    /// 管理员——新增测评分类
    /// </summary>
    /// <param name="categoryAddDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = _adminRole)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(EvaluationCategory), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> AddEvaluateCategoryAsync([FromBody] CategoryAddDto categoryAddDto)
    {
        if (categoryAddDto == null) return BadRequest();
        var entity = _mapper.Map<EvaluationCategory>(categoryAddDto);

        var addResponse = await _categoryService.AddEvaluationCategoryAsync(entity);
        if (addResponse == false)
        {
            _logger.LogError("---- administrator:id:{UserId}, name:{Name} add a category error -> old:{@old} new:{@new}",
                User.FindFirst("sub").Value, User.Identity.Name, entity, categoryAddDto);
            throw new EvaluationDomainException("新增测评类别失败");
        }

        _logger.LogInformation("---- administrator:id:{UserId}, name:{Name} add a category -> old:{@old} new:{@new}",
            User.FindFirst("sub").Value, User.Identity.Name, entity, categoryAddDto);

        if (await _redisDatabase.Database.KeyExistsAsync(_categoryListKey))
            await _redisDatabase.Database.KeyDeleteAsync(_categoryListKey);

        return CreatedAtRoute(nameof(GetEvaluateCategoryAsync), new { id = entity.CategoryId }, new { categoryId = entity.CategoryId });
    }

    /// <summary>
    /// 管理员——删除测评分类
    /// </summary>
    /// <param name="id">测评类别Id</param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = _adminRole)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> RemoveEvaluateCategoryAsync(int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        var entity = await _categoryService.GetEvaluationCategoryAsync(id);
        if (entity == null) return BadRequest();

        // 当测评分类下有测评文章时 数据库约束会导致程序出现异常
        try
        {
            var delResponse = await _categoryService.DeleteEvaluationCategoryAsync(id);
            if (delResponse == false)
            {
                _logger.LogError("---- administrator:id:{UserId}, name:{Name} delete a category error -> id:{id}",
                    User.FindFirst("sub").Value, User.Identity.Name, id);
                throw new EvaluationDomainException("删除测评分类失败");
            }

            // 删除成功同时清除Redis
            if (await _redisDatabase.Database.KeyExistsAsync(_categoryListKey))
                await _redisDatabase.Database.KeyDeleteAsync(_categoryListKey);

            _logger.LogInformation("---- administrator:id:{UserId}, name:{Name} delete a category -> id:{id}",
                User.FindFirst("sub").Value, User.Identity.Name, id);
            return NoContent();

        }
        catch (MySqlException ex)
        {
            _logger.LogError("---- administrator:id:{UserId}, name:{Name} delete a category error -> message:{Message}",
                User.FindFirst("sub").Value, User.Identity.Name, ex.Message);
            throw new EvaluationDomainException("程序错误，可能是数据库约束导致的", ex);
        }
    }
}