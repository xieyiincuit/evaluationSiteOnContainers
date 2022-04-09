namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

/// <summary>
/// 游戏标签管理接口(系统中暂时未使用该模块)
/// </summary>
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/v1/game")]
public class GameTagController : ControllerBase
{
    private const int _pageSize = 10;
    private readonly ILogger<GameTagController> _logger;
    private readonly IMapper _mapper;
    private readonly IGameTagService _tagService;

    public GameTagController(
        IGameTagService tagService,
        IMapper mapper,
        ILogger<GameTagController> logger)
    {
        _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger;
    }

    /// <summary>
    /// 用户，管理员——分页获取游戏标签信息
    /// </summary>
    /// <param name="pageIndex">pageSize=10</param>
    /// <returns></returns>
    [HttpGet("tags")]
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

    /// <summary>
    /// 用户，管理员——获取特定游戏标签
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    [HttpGet("tag/{tagId:int}", Name = nameof(GetTagByIdAsync))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GameTag), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetTagByIdAsync([FromRoute] int tagId)
    {
        if (tagId <= 0 || tagId >= int.MaxValue) return BadRequest();
        var tag = await _tagService.GetGameTagAsync(tagId);
        return tag == null ? NotFound() : Ok(tag);
    }


    [HttpPost("tag")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateTagAsync([FromBody] GameTagAddDto tagAddDto)
    {
        if (tagAddDto == null) return BadRequest();

        var entityToAdd = _mapper.Map<GameTag>(tagAddDto);
        await _tagService.AddGameTagAsync(entityToAdd);

        _logger.LogInformation(
            $"administrator: id:{User.FindFirst("sub").Value}, name:{User.FindFirst("nickname")} add a gameTag -> tagName:{tagAddDto.TagName}");
        return CreatedAtRoute(nameof(GetTagByIdAsync), new { tagId = entityToAdd.Id }, null);
    }

    [HttpDelete("tag/{id:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        _logger.LogInformation(
            $"administrator: id:{User.FindFirst("sub").Value}, name:{User.FindFirst("nickname")} delete a gameTag -> tagId:{id}");
        var response = await _tagService.DeleteGameTagAsync(id);
        return response == true ? NoContent() : NotFound();
    }

    [HttpPut("tag")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateCategoryAsync([FromBody] GameTagUpdateDto tagUpdateDto)
    {
        if (tagUpdateDto == null) return BadRequest();

        var entityToUpdate = await _tagService.GetGameTagAsync(tagUpdateDto.Id);
        if (entityToUpdate == null) return NotFound();

        _logger.LogInformation(
            $"administrator: id:{User.FindFirst("sub").Value}, name:{User.FindFirst("nickname")} update a gameTag -> old:{entityToUpdate.TagName}, new:{tagUpdateDto.TagName}");

        _mapper.Map(tagUpdateDto, entityToUpdate);
        await _tagService.UpdateGameTagAsync(entityToUpdate);
        return NoContent();
    }
}