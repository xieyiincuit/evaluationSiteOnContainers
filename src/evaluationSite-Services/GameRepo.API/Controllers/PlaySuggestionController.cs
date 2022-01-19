namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/g")]
public class PlaySuggestionController : ControllerBase
{
    private readonly IPlaySuggestionService _suggestionService;
    private readonly IMapper _mapper;
    private const int _pageSize = 10;

    public PlaySuggestionController(IPlaySuggestionService suggestionService, IMapper mapper)
    {
        _suggestionService = suggestionService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("suggestions")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<PlaySuggestion>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSuggestionsAsync([FromQuery] int pageIndex = 1)
    {
        var totalSuggestions = await _suggestionService.CountPlaySuggestionsAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalSuggestions, _pageSize, pageIndex)) pageIndex = 1;

        var suggestions = await _suggestionService.GetPlaySuggestionsAsync(pageIndex, _pageSize);
        if (!suggestions.Any()) return NotFound();

        var model = new PaginatedItemsDtoModel<PlaySuggestion>(pageIndex, _pageSize, totalSuggestions, suggestions);
        return Ok(model);
    }

    [HttpGet("suggestion/{suggestionId:int}", Name = nameof(GetSuggestionAsync))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PlaySuggestion), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSuggestionAsync(int suggestionId)
    {
        if (suggestionId <= 0 || suggestionId >= int.MaxValue) return BadRequest();

        var suggestion = await _suggestionService.GetPlaySuggestionAsync(suggestionId);

        if (suggestion == null) return NotFound();
        return Ok(suggestion);
    }

    [HttpDelete]
    [Route("suggestion/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteSuggestionAsync(int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();

        var response = await _suggestionService.DeletePlaySuggestionAsync(id);
        return response == true ? NoContent() : NotFound();
    }

    [HttpPut]
    [Route("suggestion")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateCategoryAsync([FromBody] PlaySuggestionUpdateDto suggestionUpdateDto)
    {
        if (suggestionUpdateDto == null) return BadRequest();

        var entityToUpdate = _mapper.Map<PlaySuggestion>(suggestionUpdateDto);
        await _suggestionService.UpdatePlaySuggestionAsync(entityToUpdate);
        return NoContent();
    }
}
