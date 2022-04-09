namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

/// <summary>
/// 游戏建议管理接口
/// </summary>
[ApiController]
[Route("api/v1/game")]
public class GameSuggestionController : ControllerBase
{
    private const int _pageSize = 10;
    private readonly ILogger<GameSuggestionController> _logger;
    private readonly IMapper _mapper;
    private readonly IPlaySuggestionService _suggestionService;
    private readonly IGameInfoService _gameInfoService;

    public GameSuggestionController(
        IPlaySuggestionService suggestionService,
        IGameInfoService gameInfoService,
        IMapper mapper,
        ILogger<GameSuggestionController> logger)
    {
        _suggestionService = suggestionService ?? throw new ArgumentNullException(nameof(suggestionService));
        _gameInfoService = gameInfoService ?? throw new ArgumentNullException(nameof(gameInfoService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 用户，管理员——分页获取游戏建议
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    [HttpGet("suggestions")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<PlaySuggestionDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSuggestionsAsync([FromQuery] int pageIndex = 1)
    {
        var totalSuggestions = await _suggestionService.CountPlaySuggestionsAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalSuggestions, _pageSize, pageIndex)) pageIndex = 1;

        var suggestions = await _suggestionService.GetPlaySuggestionsAsync(pageIndex, _pageSize);
        if (!suggestions.Any()) return NotFound();

        var suggestionDto = _mapper.Map<List<PlaySuggestionDto>>(suggestions);

        var model = new PaginatedItemsDtoModel<PlaySuggestionDto>(pageIndex, _pageSize, totalSuggestions, suggestionDto);
        return Ok(model);
    }

    /// <summary>
    /// 用户，管理员——获取特定游戏建议信息
    /// </summary>
    /// <param name="suggestionId">游戏建议Id</param>
    /// <returns></returns>
    [HttpGet("suggestion/{suggestionId:int}", Name = nameof(GetSuggestionByIdAsync))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GamePlaySuggestion), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSuggestionByIdAsync([FromRoute] int suggestionId)
    {
        if (suggestionId <= 0 || suggestionId >= int.MaxValue) return BadRequest();
        var suggestion = await _suggestionService.GetPlaySuggestionAsync(suggestionId);
        return suggestion == null ? NotFound() : Ok(suggestion);
    }

    /// <summary>
    /// 用户，管理员——获取特定游戏的游戏建议
    /// </summary>
    /// <param name="gameId">游戏Id</param>
    /// <returns></returns>
    [HttpGet("suggestion")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(GamePlaySuggestion), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSuggestionByGameIdAsync([FromQuery] int gameId)
    {
        if (!await _gameInfoService.GameExistAsync(gameId)) return BadRequest();
        var suggestion = await _suggestionService.GetPlaySuggestionByGameAsync(gameId) ?? new GamePlaySuggestion();
        return Ok(suggestion);
    }

    /// <summary>
    /// 管理员——为游戏添加游戏建议
    /// </summary>
    /// <param name="suggestionAddDto"></param>
    /// <returns></returns>
    [HttpPost("suggestion")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateSuggestionAsync([FromBody] PlaySuggestionAddDto suggestionAddDto)
    {
        if (suggestionAddDto == null) return BadRequest();
        if (!await _gameInfoService.GameExistAsync(suggestionAddDto.GameId)) return BadRequest();

        var entityToAdd = _mapper.Map<GamePlaySuggestion>(suggestionAddDto);
        try
        {
            var addResponse = await _suggestionService.AddPlaySuggestionAsync(entityToAdd);
            if (addResponse == false)
            {
                _logger.LogError("administrator: id:{id}, name:{Name} add a suggestion error -> suggestion:{@suggestion}",
                    User.FindFirst("sub").Value, User.FindFirst("nickname"), suggestionAddDto);
                throw new GameRepoDomainException("数据库新增游戏建议失败");
            }
        }
        catch (MySqlException ex)
        {
            _logger.LogError("administrator: id:{id}, name:{Name} add a suggestion throw a exception -> ErrorMessage:{Error}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), ex.Message);
            throw new GameRepoDomainException("数据库新增游戏建议失败, 数据库外键约束导致，请确认你新增的游戏是否存在");
        }

        _logger.LogInformation("administrator: id:{id}, name:{Name} add a suggestion -> suggestion:{@suggestion}",
            User.FindFirst("sub").Value, User.FindFirst("nickname"), suggestionAddDto);
        return CreatedAtRoute(nameof(GetSuggestionByIdAsync), new { suggestionId = entityToAdd.Id }, new { suggestionId = entityToAdd.Id });
    }

    /// <summary>
    /// 管理员——删除游戏建议
    /// </summary>
    /// <param name="id">游戏建议Id</param>
    /// <returns></returns>
    [HttpDelete("suggestion/{id:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteSuggestionAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        var entity = await _suggestionService.GetPlaySuggestionAsync(id);
        if (entity == null) return BadRequest();

        var delResponse = await _suggestionService.DeletePlaySuggestionAsync(id);
        if (delResponse == false)
        {
            _logger.LogInformation("administrator: id:{id}, name:{Name} delete a suggestion error -> suggestionId:{Id}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), id);
        }

        _logger.LogInformation("administrator: id:{id}, name:{Name} delete a suggestion -> suggestionId:{Id}",
            User.FindFirst("sub").Value, User.FindFirst("nickname"), id);
        return NoContent();
    }

    /// <summary>
    /// 管理员——修改游戏建议信息
    /// </summary>
    /// <param name="suggestionUpdateDto"></param>
    /// <returns></returns>
    [HttpPut("suggestion")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateSuggestionAsync([FromBody] PlaySuggestionUpdateDto suggestionUpdateDto)
    {
        if (suggestionUpdateDto == null) return BadRequest();

        var entityToUpdate = await _suggestionService.GetPlaySuggestionAsync(suggestionUpdateDto.Id);
        if (entityToUpdate == null) return NotFound();

        _mapper.Map(suggestionUpdateDto, entityToUpdate);
        var updateResponse = await _suggestionService.UpdatePlaySuggestionAsync(entityToUpdate);
        if (updateResponse == false)
        {
            _logger.LogError("administrator: id:{id}, name:{Name} update a suggestion error -> old:{@old} new:{@new}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), entityToUpdate, suggestionUpdateDto);
            throw new GameRepoDomainException("数据库修改游戏建议信息失败");
        }

        _logger.LogInformation("administrator: id:{id}, name:{Name} update a suggestion -> old:{@old} new:{@new}",
            User.FindFirst("sub").Value, User.FindFirst("nickname"), entityToUpdate, suggestionUpdateDto);
        return NoContent();
    }
}