namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

/// <summary>
/// 游戏建议管理
/// </summary>
[ApiController]
[Route("api/v1/game")]
public class PlaySuggestionController : ControllerBase
{
    private const int _pageSize = 10;
    private readonly ILogger<PlaySuggestionController> _logger;
    private readonly IMapper _mapper;
    private readonly IPlaySuggestionService _suggestionService;
    private readonly IGameInfoService _gameInfoService;

    public PlaySuggestionController(
        IPlaySuggestionService suggestionService,
        IGameInfoService gameInfoService,
        IMapper mapper,
        ILogger<PlaySuggestionController> logger)
    {
        _suggestionService = suggestionService ?? throw new ArgumentNullException(nameof(suggestionService));
        _gameInfoService = gameInfoService ?? throw new ArgumentNullException(nameof(gameInfoService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 分页获取游戏建议
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("suggestions")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<GamePlaySuggestion>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSuggestionsAsync([FromQuery] int pageIndex = 1)
    {
        var totalSuggestions = await _suggestionService.CountPlaySuggestionsAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalSuggestions, _pageSize, pageIndex)) pageIndex = 1;

        var suggestions = await _suggestionService.GetPlaySuggestionsAsync(pageIndex, _pageSize);
        if (!suggestions.Any()) return NotFound();

        var model = new PaginatedItemsDtoModel<GamePlaySuggestion>(pageIndex, _pageSize, totalSuggestions, suggestions);
        return Ok(model);
    }

    /// <summary>
    /// 获取游戏建议Id获取游戏建议
    /// </summary>
    /// <param name="suggestionId"></param>
    /// <returns></returns>
    [HttpGet("suggestion/{suggestionId:int}", Name = nameof(GetSuggestionByIdAsync))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GamePlaySuggestion), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSuggestionByIdAsync([FromRoute] int suggestionId)
    {
        if (suggestionId <= 0 || suggestionId >= int.MaxValue) return BadRequest();

        var suggestion = await _suggestionService.GetPlaySuggestionAsync(suggestionId);

        if (suggestion == null) return NotFound();
        return Ok(suggestion);
    }

    /// <summary>
    /// 通过游戏Id获取游戏建议
    /// </summary>
    /// <param name="gameId"></param>
    /// <returns></returns>
    [HttpGet("suggestion")]
    public async Task<IActionResult> GetSuggestionByGameIdAsync([FromQuery] int gameId)
    {
        if (!await _gameInfoService.GameExistAsync(gameId)) return BadRequest();
        var suggestion = await _suggestionService.GetPlaySuggestionByGameAsync(gameId);
        if (suggestion == null)
        {
            suggestion = new GamePlaySuggestion();
        }
        return Ok(suggestion);
    }

    /// <summary>
    /// 新增游戏建议 在新增游戏后让管理员填写游戏建议
    /// </summary>
    /// <param name="suggestionAddDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("suggestion")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateSuggestionAsync([FromBody] PlaySuggestionAddDto suggestionAddDto)
    {
        if (suggestionAddDto == null) return BadRequest();

        var entityToAdd = _mapper.Map<GamePlaySuggestion>(suggestionAddDto);
        await _suggestionService.AddPlaySuggestionAsync(entityToAdd);

        _logger.LogInformation("administrator: id:{id}, name:{Name} add a suggestion -> suggestion:{@suggestion}",
            User.FindFirst("sub").Value, User.Identity.Name, suggestionAddDto);

        return CreatedAtRoute(nameof(GetSuggestionByIdAsync), new { suggestionId = entityToAdd.Id }, null);
    }

    /// <summary>
    /// 删除游戏建议
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("suggestion/{id:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteSuggestionAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();

        _logger.LogInformation(
            $"administrator: id:{User.FindFirst("sub").Value}, name:{User.Identity.Name} delete a suggestion -> Id:{id}");
        var response = await _suggestionService.DeletePlaySuggestionAsync(id);
        return response == true ? NoContent() : NotFound();
    }

    /// <summary>
    /// 修改游戏建议
    /// </summary>
    /// <param name="suggestionUpdateDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("suggestion")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateSuggestionAsync([FromBody] PlaySuggestionUpdateDto suggestionUpdateDto)
    {
        if (suggestionUpdateDto == null) return BadRequest();

        var entityToUpdate = await _suggestionService.GetPlaySuggestionAsync(suggestionUpdateDto.Id);

        if (entityToUpdate == null) return NotFound();

        _logger.LogInformation("administrator: id:{id}, name:{Name} add a suggestion -> old:{@old} new:{@new}",
            User.FindFirst("sub").Value, User.Identity.Name, entityToUpdate, suggestionUpdateDto);
        _mapper.Map(suggestionUpdateDto, entityToUpdate);
        await _suggestionService.UpdatePlaySuggestionAsync(entityToUpdate);
        return NoContent();
    }
}