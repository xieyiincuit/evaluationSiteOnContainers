namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

/// <summary>
/// 游戏类别管理接口
/// </summary>
[ApiController]
[Route("api/v1/game")]
public class GameCategoryController : ControllerBase
{
    private const int _pageSize = 10;
    private const string _categoriesKey = "gameCategories";

    private readonly IGameCategoryService _categoryService;
    private readonly ILogger<GameCategoryController> _logger;
    private readonly IRedisDatabase _redisDatabase;
    private readonly IMapper _mapper;

    public GameCategoryController(
        IGameCategoryService categoryService,
        IMapper mapper,
        ILogger<GameCategoryController> logger,
        IRedisDatabase redisDatabase)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
    }

    /// <summary>
    /// 用户，管理员——获取所有游戏分类，可用于Select框
    /// </summary>
    /// <returns></returns>
    [HttpGet("categories/all")]
    [ProducesResponseType(typeof(List<GameCategory>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAllCategoryAsync()
    {
        if (_redisDatabase.Database.KeyExists(_categoriesKey))
        {
            var cacheCategory = await _redisDatabase.GetAsync<List<GameCategory>>(_categoriesKey);
            _logger.LogInformation("gameCategories get from redis {@categoryList}", cacheCategory);
            return Ok(cacheCategory);
        }

        var allCategories = await _categoryService.GetAllGameCategoriesAsync();
        if (!allCategories.Any())
        {
            return NotFound();
        }

        var redisResponse = await _redisDatabase.AddAsync(_categoriesKey, allCategories);
        if (redisResponse == false)
        {
            _logger.LogWarning("redis add game category {@CategoryList} Error", allCategories);
        }
        return Ok(allCategories);
    }

    /// <summary>
    /// 用户，管理员——分页获取游戏类型
    /// </summary>
    /// <param name="pageIndex">index默认为1，pageSize后台定死为10</param>
    /// <returns></returns>
    [HttpGet("categories")]
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

    /// <summary>
    /// 用户，管理员——获取特定游戏类型
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    [HttpGet("category/{categoryId:int}", Name = nameof(GetCategoryAsync))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GameCategory), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategoryAsync([FromRoute] int categoryId)
    {
        if (categoryId <= 0 || categoryId >= int.MaxValue) return BadRequest();

        var category = await _categoryService.GetGameCategoryAsync(categoryId);

        return category == null ? NotFound() : Ok(category);
    }

    /// <summary>
    /// 管理员——新增游戏类型
    /// </summary>
    /// <param name="categoryAddDto"></param>
    /// <returns></returns>
    [HttpPost("category")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] GameCategoryAddDto categoryAddDto)
    {
        if (categoryAddDto == null) return BadRequest();

        if (await _categoryService.HasSameCategoryNameAsync(categoryAddDto.CategoryName))
            return BadRequest("该游戏类型已存在");
        
        var entityToAdd = _mapper.Map<GameCategory>(categoryAddDto);
        var addResponse = await _categoryService.AddCategoryAsync(entityToAdd);
        if (addResponse == false)
        {
            _logger.LogError("---- administrator:id:{UserId}, name:{Name} add a category error -> new:{@new}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), categoryAddDto);
            throw new GameRepoDomainException("数据库新增游戏类型失败");
        }
        else
        {
            _logger.LogInformation("administrator: id:{UserId}, name:{UserName} add a category -> new:{@new}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), categoryAddDto);
            // 新增数据后删除旧数据缓存
            if (await _redisDatabase.Database.KeyExistsAsync(_categoriesKey))
            {
                var redisResponse = await _redisDatabase.Database.KeyDeleteAsync(_categoriesKey);
                if (redisResponse == false)
                {
                    _logger.LogError("redis delete game category error when game category add a new one");
                    throw new GameRepoDomainException("数据库新增游戏分类时，Redis缓存删除失败，请手动删除缓存防止出现错误数据");
                }
            }
            return CreatedAtRoute(nameof(GetCategoryAsync), new { categoryId = entityToAdd.Id }, new { categoryId = entityToAdd.Id });
        }
    }

    /// <summary>
    /// 管理员——删除游戏类型(该游戏类型下已新增游戏，删除会被数据库约束阻止)
    /// </summary>
    /// <param name="id">游戏类型Id</param>
    /// <returns></returns>
    [HttpDelete("category/{id:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        var entity = await _categoryService.GetGameCategoryAsync(id);
        if (entity == null) return BadRequest();

        try
        {
            var delResponse = await _categoryService.DeleteCategoryAsync(id);
            if (delResponse == false)
            {
                _logger.LogError("administrator: id:{UserId}, name:{UserName} delete a category -> categoryId:{id}",
                    User.FindFirst("sub").Value, User.FindFirst("nickname"), id);
                throw new GameRepoDomainException("数据库删除游戏分类失败");
            }

            // 删除成功 一并清除缓存
            if (await _redisDatabase.Database.KeyExistsAsync(_categoriesKey))
            {
                var redisResponse = await _redisDatabase.Database.KeyDeleteAsync(_categoriesKey);
                if (redisResponse == false)
                {
                    _logger.LogError("redis delete game category error when delete a category");
                    throw new GameRepoDomainException("数据库删除游戏分类时，Redis缓存删除失败，请手动删除缓存防止出现错误数据");
                }
            }

            return NoContent();
        }
        catch (MySqlException ex)
        {
            _logger.LogError("---- administrator:id:{UserId}, name:{Name} delete a category error -> message:{Message}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), ex.Message);
            throw new GameRepoDomainException("程序错误，可能是数据库约束导致的", ex);
        }
    }

    /// <summary>
    /// 管理员——修改游戏类型名
    /// </summary>
    /// <param name="categoryUpdateDto"></param>
    /// <returns></returns>
    [HttpPut("category")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateCategoryAsync([FromBody] GameCategoryUpdateDto categoryUpdateDto)
    {
        if (categoryUpdateDto == null) return BadRequest();

        var entityToUpdate = await _categoryService.GetGameCategoryAsync(categoryUpdateDto.Id);
        if (entityToUpdate == null) return NotFound();

        if (await _categoryService.HasSameCategoryNameAsync(categoryUpdateDto.CategoryName))
            return BadRequest("该游戏类型已存在");
        
        _logger.LogInformation("---- administrator:id:{UserId}, name:{Name} update a category -> old:{@old} new:{@new}",
            User.FindFirst("sub").Value, User.FindFirst("nickname"), entityToUpdate, categoryUpdateDto);

        _mapper.Map(categoryUpdateDto, entityToUpdate);
        var updateResponse = await _categoryService.UpdateCategoryAsync(entityToUpdate);
        if (updateResponse == false)
        {
            _logger.LogError("---- administrator:id:{UserId}, name:{Name} update a category error -> old:{@old} new:{@new}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), entityToUpdate, categoryUpdateDto);
            throw new GameRepoDomainException("数据库修改游戏分类失败");
        }

        // 修改成功,清除缓存同步数据
        if (await _redisDatabase.Database.KeyExistsAsync(_categoriesKey))
        {
            var redisResponse = await _redisDatabase.Database.KeyDeleteAsync(_categoriesKey);
            if (redisResponse == false)
            {
                _logger.LogError("redis delete game category error when delete a category");
                throw new GameRepoDomainException("数据库更新游戏分类时，Redis缓存删除失败，请手动删除缓存防止出现错误数据");
            }
        }
        return NoContent();
    }
}