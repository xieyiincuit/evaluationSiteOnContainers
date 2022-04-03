namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

/// <summary>
/// 游戏信息管理
/// </summary>
[ApiController]
[Route("api/v1/game")]
public class GameInfoController : ControllerBase
{
    private const int _pageSize = 18;
    private readonly IGameInfoService _gameInfoService;
    private readonly ILogger<GameInfoController> _logger;
    private readonly IMapper _mapper;
    private readonly IGameRepoIntegrationEventService _repoIntegrationEventService;
    private readonly IUnitOfWorkService _unitOfWorkService;

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
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<GameInfoSmallDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetGameInfosAsync(
        [FromQuery] int? categoryId, [FromQuery] int? companyId,
        [FromQuery] int pageIndex = 1, [FromQuery] string order = "hot")
    {
        var totalGames = await _gameInfoService.CountGameInfoAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalGames, _pageSize, pageIndex)) pageIndex = 1;

        var games = await _gameInfoService.GetGameInfoWithTermAsync(pageIndex, _pageSize, categoryId, companyId, order);

        if (!games.Any()) return NotFound();
        var gamesDto = _mapper.Map<List<GameInfoSmallDto>>(games);

        var model = new PaginatedItemsDtoModel<GameInfoSmallDto>(pageIndex, _pageSize, totalGames, gamesDto);
        return Ok(model);
    }

    /// <summary>
    /// 管理员获取游戏信息
    /// </summary>
    /// <param name="categoryId">可根据游戏类别筛选</param>
    /// <param name="companyId">可根据游戏公司筛选</param>
    /// <param name="pageIndex">pageSize固定为10</param>
    /// <param name="order">排序方式：hot-游戏热度倒序(默认) time-发售时间倒序 score-游戏评分倒序 (其他无效字段)-Id倒序</param>
    /// <returns></returns>
    [HttpGet]
    [Route("admin/infos")]
    //[Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<GameInfoAdminDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetGameInfoForAdminAsync(
        [FromQuery] int? categoryId, [FromQuery] int? companyId,
        [FromQuery] int pageIndex = 1, [FromQuery] string? order = "hot")
    {
        int pageIndexForAdmin = 10;
        var totalGames = await _gameInfoService.CountGameInfoAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalGames, pageIndexForAdmin, pageIndex)) pageIndex = 1;

        var games = await _gameInfoService.GetGameInfoWithTermAsync(pageIndex, pageIndexForAdmin, categoryId, companyId, order);

        if (!games.Any()) return NotFound();
        var gamesDto = _mapper.Map<List<GameInfoAdminDto>>(games);

        var model = new PaginatedItemsDtoModel<GameInfoAdminDto>(pageIndex, pageIndexForAdmin, totalGames, gamesDto);
        return Ok(model);
    }

    [HttpGet]
    [Route("ranks")]
    public async Task<IActionResult> GetGameRankAsync()
    {
        var games = await _gameInfoService.GetGameInfoRankAsync();
        return !games.Any() ? NotFound() : Ok(games);
    }

    /// <summary>
    /// 获取所有游戏列表，可用于测评文章选择和商品上架游戏选择
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("selectlist")]
    public async Task<IActionResult> GetGameSelectListAsync()
    {
        var games = await _gameInfoService.GetGameSelectAsync();
        return !games.Any() ? NotFound() : Ok(games);
    }

    /// <summary>
    /// 获取特定游戏具体信息
    /// </summary>
    /// <param name="gameId"></param>
    /// <returns></returns>
    [HttpGet("info/{gameId:int}", Name = nameof(GetGameInfoByIdAsync))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(GameInfoSmallDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetGameInfoByIdAsync([FromRoute] int gameId)
    {
        if (gameId <= 0 || gameId >= int.MaxValue) return BadRequest();

        var game = await _gameInfoService.GetGameInfoAsync(gameId);
        if (game == null) return NotFound();

        var gameDto = _mapper.Map<GameInfoDto>(game);
        return Ok(gameDto);
    }

    /// <summary>
    /// 新增游戏信息
    /// </summary>
    /// <param name="gameInfoAddDto">supportPlatform需校验 内容用/隔开： 如PC/PS4</param>
    /// <returns></returns>
    [HttpPost] //定义该Action为HTTP POST
    [Route("info")] //定义子路由
    [Authorize(Roles = "administrator")] //定义该方法需要身份验证且授权给administrator用户
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateGameInfoAsync([FromBody] GameInfoAddDto gameInfoAddDto) //规定参数从HTTP Body中接受
    {
        //若未带参数请求该接口 直接返回400
        if (gameInfoAddDto == null) return BadRequest();

        //将DTO映射为游戏信息数据库实体
        var entityToAdd = _mapper.Map<GameInfo>(gameInfoAddDto);

        //记录日志
        _logger.LogInformation($"administrator: id:{User.FindFirst("sub").Value}, name:{User.Identity.Name} add a gameInfo -> GameName:{gameInfoAddDto.Name}");

        //调用Service将游戏实体写入数据库
        await _gameInfoService.AddGameInfoAsync(entityToAdd);

        //工作单元保存
        await _unitOfWorkService.SaveChangesAsync();
        return CreatedAtRoute(nameof(GetGameInfoByIdAsync), new { gameId = entityToAdd.Id }, null);
    }

    /// <summary>
    /// 修改游戏信息
    /// </summary>
    /// <param name="gameInfoUpdateDto">supportPlatform需校验 内容用/隔开： 如PC/PS4</param>
    /// <returns></returns>
    [HttpPut]
    [Route("info")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateGameInfoAsync([FromBody] GameInfoUpdateDto gameInfoUpdateDto)
    {
        if (gameInfoUpdateDto == null) return BadRequest();

        var gameItem = await _gameInfoService.GetGameInfoAsync(gameInfoUpdateDto.Id);
        if (gameItem == null) return NotFound(new { Message = $"game with id {gameInfoUpdateDto.Id} not fount." });

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
            var nameChangedEvent = new GameNameChangedIntegrationEvent(gameItem.Id, oldName, gameInfoUpdateDto.Name);
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


    /// <summary>
    /// 游戏管理不需要删除功能 提供修改即可
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("info/{id:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteGameInfoAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();

        await _gameInfoService.RemoveGameInfoAsync(id);
        var response = await _unitOfWorkService.SaveEntitiesAsync();
        _logger.LogInformation(
            $"administrator: id:{User.FindFirst("sub").Value}, name:{User.Identity.Name} delete a gameInfo -> Id:{id}");
        return response == true ? NoContent() : NotFound();
    }
}