namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/g")]
public class GameTagController : ControllerBase
{
    private readonly IGameTagService _tagService;
    private readonly IMapper _mapper;
    private const int _pageSize = 10;
    public GameTagController(IGameTagService tagService, IMapper mapper)
    {
        _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    [Route("tags")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<GameTag>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetTagsAsync([FromQuery] int pageIndex = 1)
    {
        var totalTags = await _tagService.CountTagsAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalTags, _pageSize, pageIndex)) pageIndex = 1;

        var tags = await _tagService.GetGameTagsAsync(pageIndex, _pageSize);
        if (!tags.Any()) return NotFound();

        var model = new PaginatedItemsDtoModel<GameTag>(pageIndex, _pageSize, totalTags, tags);
        return Ok(model);
    }

    [HttpGet("tag/{tagId:int}", Name = nameof(GetTagByIdAsync))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GameTag), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetTagByIdAsync([FromRoute] int tagId)
    {
        if (tagId <= 0 || tagId >= int.MaxValue) return BadRequest();

        var tag = await _tagService.GetGameTagAsync(tagId);

        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [HttpPost]
    [Route("tag")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateTagAsync([FromBody] GameTagAddDto tagAddDto)
    {
        if (tagAddDto == null) return BadRequest();

        var entityToAdd = _mapper.Map<GameTag>(tagAddDto);

        await _tagService.AddGameTagAsync(entityToAdd);
        return CreatedAtRoute(nameof(GetTagByIdAsync), new { tagId = entityToAdd.Id }, null);
    }

    [HttpDelete]
    [Route("tag/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();

        var response = await _tagService.DeleteGameTagAsync(id);
        return response == true ? NoContent() : NotFound();
    }

    [HttpPut]
    [Route("tag")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateCategoryAsync([FromBody] GameTagUpdateDto tagUpdateDto)
    {
        if (tagUpdateDto == null) return BadRequest();

        var entityToUpdate = _mapper.Map<GameTag>(tagUpdateDto);
        await _tagService.UpdateGameTagAsync(entityToUpdate);
        return NoContent();
    }
}
