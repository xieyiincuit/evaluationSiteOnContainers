namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/g")]
public class GameInfoController : ControllerBase
{
    private readonly IGameInfoService _gameInfoService;
    private readonly IMapper _mapper;
    private readonly ILogger<GameInfoController> _logger;
    private readonly IGameRepoIntegrationEventService _repoIntegrationEventService;
    private readonly IUnitOfWorkService _unitOfWorkService;
    private const int _pageSize = 20;

    public GameInfoController(
        IGameInfoService gameInfoService,
        IMapper mapper,
        ILogger<GameInfoController> logger,
        IGameRepoIntegrationEventService repoIntegrationEventService,
        IUnitOfWorkService unitOfWorkService)
    {
        _gameInfoService = gameInfoService ?? throw new ArgumentNullException(nameof(gameInfoService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repoIntegrationEventService = repoIntegrationEventService ?? throw new ArgumentNullException(nameof(repoIntegrationEventService));
        _unitOfWorkService = unitOfWorkService ?? throw new ArgumentNullException(nameof(unitOfWorkService));
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

        var entityToAdd = _mapper.Map<GameInfo>(gameInfoAddDto);

        await _gameInfoService.AddGameInfoAsync(entityToAdd);
        await _unitOfWorkService.SaveChangesAsync();
        return CreatedAtRoute(nameof(GetGameInfoByIdAsync), new { gameId = entityToAdd.Id }, null);
    }

    [HttpPut]
    [Route("info")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateGameInfoAsync([FromBody] GameInfoUpdateDto gameInfoUpdateDto)
    {
        if (gameInfoUpdateDto == null) return BadRequest();

        var gameItem = await _gameInfoService.GetGameInfoAsync(gameInfoUpdateDto.Id);
        if (gameItem == null)
        {
            return NotFound(new { Message = $"game with id {gameInfoUpdateDto.Id} not fount." });
        }

        //检查是否更新了游戏名，若更新需要发布集成事件通知其他相关服务
        var oldName = gameItem.Name;
        var raiseGameNameChangedEvent = oldName != gameInfoUpdateDto.Name;

        //更新游戏信息
        _mapper.Map(gameInfoUpdateDto, gameItem);
        await _gameInfoService.UpdateGameInfoAsync(gameItem);

        if (raiseGameNameChangedEvent)
        {
            _logger.LogInformation("----- GameNameChangedEvent Raised, Will Send a message to Event Bus");
            //1. 初始化集成事件，待事件总线发布。
            var nameChangedEvent = new GameNameChangedIntegrationEvent(gameItem.Id, oldName: oldName, newName: gameInfoUpdateDto.Name);
            //2. 使用事务保证原子性的同时，在发布事件时同时记录事件日志。
            await _repoIntegrationEventService.SaveEventAndGameRepoContextChangeAsync(nameChangedEvent);
            //3. 将该事件发布并修改该事件发布状态为已发布
            await _repoIntegrationEventService.PublishThroughEventBusAsync(nameChangedEvent);
        }
        else
        {
            // Just save the updated gameInfo because the Game's Name hasn't changed.
            await _unitOfWorkService.SaveEntitiesAsync();
        }

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

        await _gameInfoService.RemoveGameInfoAsync(id);
        var response = await _unitOfWorkService.SaveEntitiesAsync();
        return response == true ? NoContent() : NotFound();
    }
}
