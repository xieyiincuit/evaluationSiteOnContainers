namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

/// <summary>
/// 游戏信息管理接口
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

    /// <summary>
    /// 用户——获取游戏展示信息
    /// </summary>
    /// <param name="categoryId">游戏类型</param>
    /// <param name="companyId">游戏公司</param>
    /// <param name="pageIndex">pageSize=18</param>
    /// <param name="order">hot-热度排序 time-发布事件排序 score-游戏评分排序</param>
    /// <returns></returns>
    [HttpGet("infos")]
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
    /// 管理员——获取游戏信息列表
    /// </summary>
    /// <param name="categoryId">可根据游戏类别筛选</param>
    /// <param name="companyId">可根据游戏公司筛选</param>
    /// <param name="pageIndex">pageSize固定为10</param>
    /// <param name="order">排序方式：hot-游戏热度倒序(默认) time-发售时间倒序 score-游戏评分倒序 (其他无效字段)-Id倒序</param>
    /// <returns></returns>
    [HttpGet("admin/infos")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<GameInfoAdminDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetGameInfoForAdminAsync(
        [FromQuery] int? categoryId, [FromQuery] int? companyId,
        [FromQuery] int pageIndex = 1, [FromQuery] string? order = "hot")
    {
        const int pageIndexForAdmin = 10;
        var totalGames = await _gameInfoService.CountGameInfoWithTermAsync(categoryId, companyId);
        if (ParameterValidateHelper.IsInvalidPageIndex(totalGames, pageIndexForAdmin, pageIndex)) pageIndex = 1;

        var games = await _gameInfoService.GetGameInfoWithTermAsync(pageIndex, pageIndexForAdmin, categoryId, companyId, order);

        if (!games.Any()) return NotFound();
        var gamesDto = _mapper.Map<List<GameInfoAdminDto>>(games);

        var model = new PaginatedItemsDtoModel<GameInfoAdminDto>(pageIndex, pageIndexForAdmin, totalGames, gamesDto);
        return Ok(model);
    }

    /// <summary>
    /// 用户——获取评分排名前十的游戏
    /// </summary>
    /// <returns></returns>
    [HttpGet("ranks")]
    [ProducesResponseType(typeof(List<GameRankDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetGameRankAsync()
    {
        var games = await _gameInfoService.GetGameInfoRankAsync();
        return !games.Any() ? NotFound() : Ok(games);
    }

    /// <summary>
    /// 用户，管理员——获取所有游戏列表，用于测评文章选择和商品上架游戏选择
    /// </summary>
    /// <returns></returns>
    [HttpGet("selectlist")]
    [ProducesResponseType(typeof(List<GameSelectDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetGameSelectListAsync()
    {
        var games = await _gameInfoService.GetGameSelectAsync();
        return !games.Any() ? NotFound() : Ok(games);
    }

    /// <summary>
    /// 用户，管理员——获取游戏的具体信息
    /// </summary>
    /// <param name="gameId">游戏Id</param>
    /// <returns></returns>
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

    /// <summary>
    /// 管理员——新增游戏信息
    /// </summary>
    /// <param name="gameInfoAddDto">supportPlatform需校验 内容用/隔开： 如PC/PS4</param>
    /// <returns></returns>
    [HttpPost("info")] //定义该Action为HTTP POST, 定义子路由
    [Authorize(Roles = "administrator")] //定义该方法需要身份验证且授权给administrator用户
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateGameInfoAsync([FromBody] GameInfoAddDto gameInfoAddDto) //规定参数从HTTP Body中接受
    {
        //若未带参数请求该接口 直接返回400
        if (gameInfoAddDto == null) return BadRequest();

        if (await _gameInfoService.HasSameGameNameAsync(gameInfoAddDto.Name))
            return BadRequest("该游戏信息已经存在");

        //将DTO映射为游戏信息数据库实体
        var entityToAdd = _mapper.Map<GameInfo>(gameInfoAddDto);
        //记录行为日志
        _logger.LogInformation("administrator: id:{UserId}, name:{UserName} start add a gameInfo:{@new}", User.FindFirstValue("sub"), User.FindFirst("nickname"), gameInfoAddDto);
        //调用Service将游戏实体写入数据库
        await _gameInfoService.AddGameInfoAsync(entityToAdd);
        //工作单元保存
        var saveResponse = await _unitOfWorkService.SaveEntitiesAsync();
        //保存失败
        if (saveResponse == false)
        {
            //记录错误日志
            _logger.LogInformation("administrator: id:{UserId}, name:{UserName} add a gameInfo:{@new} error", User.FindFirstValue("sub"), User.FindFirst("nickname"), gameInfoAddDto);
            throw new GameRepoDomainException("数据库游戏信息添加失败");
        }
        return CreatedAtRoute(nameof(GetGameInfoByIdAsync), new { gameId = entityToAdd.Id }, new { gameId = entityToAdd.Id });
    }

    /// <summary>
    /// 管理员——修改游戏信息
    /// </summary>
    /// <param name="gameInfoUpdateDto">supportPlatform需校验 内容用/隔开： 如PC/PS4</param>
    /// <returns></returns>
    [HttpPut("info")]
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

        try
        {
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
        }
        catch (Exception ex)
        {
            _logger.LogError("administrator change gameInfo error -> gameInfo:{@game} | ErrorMessage:{Message}", gameInfoUpdateDto, ex.Message);
            throw new GameRepoDomainException("数据库更新游戏信息失败, 或集成事件发布失败，请检查日志。");
        }

        return NoContent();
    }


    /// <summary>
    /// 管理员——删除游戏信息
    /// </summary>
    /// <param name="id">游戏Id</param>
    /// <returns></returns>
    [HttpDelete("info/{id:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteGameInfoAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();
        try
        {
            await _gameInfoService.RemoveGameInfoAsync(id);
            await _unitOfWorkService.SaveEntitiesAsync();
            _logger.LogInformation("administrator delete gameInfo success -> gameInfoId:{Id}", id);
        }
        catch (MySqlException ex)
        {
            _logger.LogError("administrator delete gameInfo error -> gameInfoId:{Id} | ErrorMessage:{Message}", id, ex.Message);
            throw new GameRepoDomainException("数据库删除游戏信息失败, 外键约束导致的。");
        }

        return NoContent();
    }
}