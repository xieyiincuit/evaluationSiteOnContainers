namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

/// <summary>
/// 游戏公司管理接口
/// </summary>
[ApiController]
[Route("api/v1/game")]
public class GameCompanyController : ControllerBase
{
    private const int _pageSize = 10;
    private const string _companiesKey = "gameCompanies";
    private readonly IGameCompanyService _companyService;
    private readonly ILogger<GameCompany> _logger;
    private readonly IRedisDatabase _redisDatabase;
    private readonly IMapper _mapper;

    public GameCompanyController(
        IGameCompanyService companyService,
        IMapper mapper,
        ILogger<GameCompany> logger,
        IRedisDatabase redisDatabase)
    {
        _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
    }

    /// <summary>
    /// 用户，管理员——获取所有游戏公司，可用于Select框
    /// </summary>
    /// <returns></returns>
    [HttpGet("companies/all")]
    [ProducesResponseType(typeof(List<GameCompany>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAllCompaniesAsync()
    {
        if (_redisDatabase.Database.KeyExists(_companiesKey))
        {
            var cacheCompanies = await _redisDatabase.GetAsync<List<GameCompany>>(_companiesKey);
            _logger.LogInformation("gameCompanies get from redis {@CompanyList}", cacheCompanies);
            return Ok(cacheCompanies);
        }

        var allCompanies = await _companyService.GetAllGameCompaniesAsync();
        if (!allCompanies.Any())
        {
            return NotFound();
        }

        var redisResponse = await _redisDatabase.AddAsync(_companiesKey, allCompanies);
        if (redisResponse == false)
        {
            _logger.LogWarning("redis add game category {@CompanyList} Error", allCompanies);
        }
        return Ok(allCompanies);
    }

    /// <summary>
    /// 用户，管理员——分页获取游戏发行公司信息
    /// </summary>
    /// <param name="pageIndex">index默认为1, size后台固定为10</param>
    /// <returns></returns>
    [HttpGet("companies")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<GameCompany>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCompaniesAsync([FromQuery] int pageIndex = 1)
    {
        var totalCompanies = await _companyService.CountCompanyAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalCompanies, _pageSize, pageIndex)) pageIndex = 1;

        var companies = await _companyService.GetGameCompaniesAsync(pageIndex, _pageSize);
        if (!companies.Any()) return NotFound();

        var model = new PaginatedItemsDtoModel<GameCompany>(pageIndex, _pageSize, totalCompanies, companies);
        return Ok(model);
    }

    /// <summary>
    /// 用户，管理员——获取特定游戏公司信息
    /// </summary>
    /// <param name="companyId">公司Id</param>
    /// <returns></returns>
    [HttpGet("company/{companyId:int}", Name = nameof(GetCompanyByIdAsync))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GameCompany), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCompanyByIdAsync([FromRoute] int companyId)
    {
        if (companyId <= 0 || companyId >= int.MaxValue) return BadRequest();
        var company = await _companyService.GetGameCompanyAsync(companyId);
        return company == null ? NotFound() : Ok(company);
    }

    /// <summary>
    /// 管理员——新增游戏公司信息
    /// </summary>
    /// <param name="companyAddDto"></param>
    /// <returns></returns>
    [HttpPost("company")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateCompanyAsync([FromBody] GameCompanyAddDto companyAddDto)
    {
        if (companyAddDto == null) return BadRequest();

        if (await _companyService.HasSameCompanyNameAsync(companyAddDto.CompanyName))
            return BadRequest("该游戏公司已存在");

        var entityToAdd = _mapper.Map<GameCompany>(companyAddDto);
        var addResponse = await _companyService.AddGameCompanyAsync(entityToAdd);
        if (addResponse == false)
        {
            _logger.LogError("---- administrator:id:{UserId}, name:{Name} add a company error -> new:{@new}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), companyAddDto);
            throw new GameRepoDomainException("数据库新增游戏公司失败");
        }
        else
        {
            _logger.LogInformation("administrator: id:{UserId}, name:{UserName} add a company -> new:{@new}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), companyAddDto);
            // 新增数据后删除旧数据缓存
            if (await _redisDatabase.Database.KeyExistsAsync(_companiesKey))
            {
                var redisResponse = await _redisDatabase.Database.KeyDeleteAsync(_companiesKey);
                if (redisResponse == false)
                {
                    _logger.LogError("redis delete game company error when game company add a new one");
                    throw new GameRepoDomainException("数据库新增游戏公司时，Redis缓存删除失败，请手动删除缓存防止出现错误数据");
                }
            }
            return CreatedAtRoute(nameof(GetCompanyByIdAsync), new { companyId = entityToAdd.Id }, new { companyId = entityToAdd.Id });
        }
    }

    /// <summary>
    /// 管理员——删除游戏发行信息
    /// </summary>
    /// <param name="id">游戏公司Id</param>
    /// <returns></returns>
    [HttpDelete("company/{id:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCompanyAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        var entity = await _companyService.GetGameCompanyAsync(id);
        if (entity == null) return BadRequest();

        try
        {
            var delResponse = await _companyService.DeleteGameCompanyAsync(id);
            if (delResponse == false)
            {
                _logger.LogError("administrator: id:{UserId}, name:{UserName} delete a company -> companyId:{id}",
                    User.FindFirst("sub").Value, User.FindFirst("nickname"), id);
                throw new GameRepoDomainException("数据库删除游戏公司失败");
            }

            // 删除成功 一并清除缓存
            if (await _redisDatabase.Database.KeyExistsAsync(_companiesKey))
            {
                var redisResponse = await _redisDatabase.Database.KeyDeleteAsync(_companiesKey);
                if (redisResponse == false)
                {
                    _logger.LogError("redis delete game companies error when delete a company");
                    throw new GameRepoDomainException("数据库删除游戏公司时，Redis缓存删除失败，请手动删除缓存防止出现错误数据");
                }
            }

            return NoContent();
        }
        catch (MySqlException ex)
        {
            _logger.LogError("---- administrator:id:{UserId}, name:{Name} delete a company error -> message:{Message}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), ex.Message);
            throw new GameRepoDomainException("程序错误，可能是数据库约束导致的", ex);
        }
    }

    /// <summary>
    /// 管理员——修改游戏公司信息
    /// </summary>
    /// <param name="companyUpdateDto"></param>
    /// <returns></returns>
    [HttpPut("company")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateCompanyAsync([FromBody] GameCompanyUpdateDto companyUpdateDto)
    {
        if (companyUpdateDto == null) return BadRequest();

        var entityToUpdate = await _companyService.GetGameCompanyAsync(companyUpdateDto.Id);
        if (entityToUpdate == null) return NotFound();

        if (await _companyService.HasSameCompanyNameAsync(companyUpdateDto.CompanyName))
            return BadRequest("该游戏公司已存在");

        _logger.LogInformation("---- administrator:id:{UserId}, name:{Name} update a company -> old:{@old} new:{@new}",
            User.FindFirst("sub").Value, User.FindFirst("nickname"), entityToUpdate, companyUpdateDto);

        _mapper.Map(companyUpdateDto, entityToUpdate);
        var updateResponse = await _companyService.UpdateGameCompanyAsync(entityToUpdate);
        if (updateResponse == false)
        {
            _logger.LogError("---- administrator:id:{UserId}, name:{Name} update a company error -> old:{@old} new:{@new}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), entityToUpdate, companyUpdateDto);
            throw new GameRepoDomainException("数据库修改游戏公司失败");
        }

        // 修改成功,清除缓存同步数据
        if (await _redisDatabase.Database.KeyExistsAsync(_companiesKey))
        {
            var redisResponse = await _redisDatabase.Database.KeyDeleteAsync(_companiesKey);
            if (redisResponse == false)
            {
                _logger.LogError("redis delete game company error when delete a category");
                throw new GameRepoDomainException("数据库更新游戏公司时，Redis缓存删除失败，请手动删除缓存防止出现错误数据");
            }
        }
        return NoContent();
    }
}