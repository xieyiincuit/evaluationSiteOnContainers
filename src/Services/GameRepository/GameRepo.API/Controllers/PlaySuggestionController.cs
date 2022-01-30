namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/g")]
public class PlaySuggestionController : ControllerBase
{
    private readonly IPlaySuggestionService _suggestionService;
    private readonly IMapper _mapper;
    private readonly ILogger<PlaySuggestionController> _logger;
    private const int _pageSize = 10;

    public PlaySuggestionController(
        IPlaySuggestionService suggestionService,
        IMapper mapper,
        ILogger<PlaySuggestionController> logger)
    {
        _suggestionService = suggestionService ?? throw new ArgumentNullException(nameof(suggestionService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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

    [HttpDelete]
    [Route("suggestion/{id:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteSuggestionAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();

        _logger.LogInformation($"administrator: id:{User.FindFirst("sub").Value}, name:{User.Identity.Name} delete a suggestion -> Id:{id}");
        var response = await _suggestionService.DeletePlaySuggestionAsync(id);
        return response == true ? NoContent() : NotFound();
    }


    [HttpPut]
    [Route("suggestion")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateSuggestionAsync([FromBody] PlaySuggestionUpdateDto suggestionUpdateDto)
    {
        if (suggestionUpdateDto == null) return BadRequest();

        var entityToUpdate = await _suggestionService.GetPlaySuggestionAsync(suggestionUpdateDto.Id);

        if (entityToUpdate == null)
        {
            return NotFound();
        }

        _logger.LogInformation("administrator: id:{id}, name:{Name} add a suggestion -> old:{@old} new:{@new}",
            User.FindFirst("sub").Value, User.Identity.Name, entityToUpdate, suggestionUpdateDto);
        _mapper.Map(suggestionUpdateDto, entityToUpdate);
        await _suggestionService.UpdatePlaySuggestionAsync(entityToUpdate);
        return NoContent();
    }
}
