namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/g")]
public class GameInfoController : ControllerBase
{
    private readonly IGameInfoService _gameInfoService;
    private readonly IMapper _mapper;
    private const int _pageSize = 20;

    public GameInfoController(IGameInfoService gameInfoService, IMapper mapper)
    {
        _gameInfoService = gameInfoService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("infos")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<GameInfoDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetGameInfosAsync([FromQuery] int pageIndex = 1)
    {
        var totalGames = await _gameInfoService.CountGameInfoAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalGames, _pageSize, pageIndex)) pageIndex = 1;

        var games = await _gameInfoService.GetGameInfosAsync(pageIndex, _pageSize);
        if (!games.Any()) return NotFound();

        var gamesDto = _mapper.Map<List<GameInfoDto>>(games);

        var model = new PaginatedItemsDtoModel<GameInfoDto>(pageIndex, _pageSize, totalGames, gamesDto);
        return Ok(model);
    }

    [HttpGet("info/{gameId:int}", Name = nameof(GetGameInfoByIdAsync))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(GameInfoDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetGameInfoByIdAsync([FromRoute] int gameId)
    {
        if (gameId <= 0 || gameId >= int.MaxValue) return BadRequest();

        var game = await _gameInfoService.GetGameInfoAsync(gameId);
        if (game == null) return NotFound();

        var gameDto = _mapper.Map<GameInfoDto>(game);
        return Ok(gameDto);
    }

    [HttpPost]
    [Route("info")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateGameInfoAsync([FromBody] GameInfoAddDto gameInfoAddDto)
    {
        if (gameInfoAddDto == null) return BadRequest();

        var entityToadd = _mapper.Map<GameInfo>(gameInfoAddDto);

        await _gameInfoService.AddGameInfoAsync(entityToadd);
        return CreatedAtRoute(nameof(GetGameInfoByIdAsync), new { gameId = entityToadd.Id }, null);
    }

    [HttpPut]
    [Route("info")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateGameInfoAsync([FromBody] GameInfoUpdateDto gameInfoUpdateDto)
    {
        if (gameInfoUpdateDto == null) return BadRequest();

        var entityToUpdate = _mapper.Map<GameInfo>(gameInfoUpdateDto);
        await _gameInfoService.UpdateGameInfoAsync(entityToUpdate);
        return NoContent();
    }

    [HttpDelete]
    [Route("info/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteGameInfoAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();

        var response = await _gameInfoService.RemoveGameInfoAsync(id);
        if (response == false) return NotFound();

        return NoContent();
    }
}
