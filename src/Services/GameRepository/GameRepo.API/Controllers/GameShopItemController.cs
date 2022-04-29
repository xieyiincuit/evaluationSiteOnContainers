namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

/// <summary>
/// 游戏商品管理接口
/// </summary>
[ApiController]
[Route("api/v1")]
public class GameShopItemController : ControllerBase
{
    private const int _pageSize = 10;
    private readonly ILogger<GameShopItemController> _logger;
    private readonly IMapper _mapper;
    private readonly IRedisDatabase _redisDatabase;
    private readonly IDistributedLockFactory _distributedLockFactory;
    private readonly EvaluationCallService _evaluationService;
    private readonly IGameItemSDKService _sdkService;
    private readonly IGameShopItemService _shopItemService;
    private readonly IUnitOfWorkService _unitOfWorkService;

    public GameShopItemController(
        IGameShopItemService shopItemService,
        IGameItemSDKService sdkService,
        IUnitOfWorkService unitOfWorkService,
        IMapper mapper,
        ILogger<GameShopItemController> logger,
        IRedisDatabase redisDatabase,
        IDistributedLockFactory distributedLockFactory,
        EvaluationCallService evaluationService)
    {
        _shopItemService = shopItemService ?? throw new ArgumentNullException(nameof(shopItemService));
        _sdkService = sdkService ?? throw new ArgumentNullException(nameof(sdkService));
        _unitOfWorkService = unitOfWorkService ?? throw new ArgumentNullException(nameof(unitOfWorkService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
        _distributedLockFactory = distributedLockFactory ?? throw new ArgumentNullException(nameof(distributedLockFactory));
        _evaluationService = evaluationService ?? throw new ArgumentNullException(nameof(evaluationService));
    }

    /// <summary>
    /// 管理员——获取商品信息列表
    /// </summary>
    /// <param name="pageIndex">分页大小定死为10</param>
    /// <returns></returns>
    [HttpGet("game/shops")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<ShopItemDtoToAdmin>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetShopItemsToAdminAsync([FromQuery] int pageIndex = 1)
    {
        var shopItemsCount = await _shopItemService.CountGameShopItemAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(shopItemsCount, _pageSize, pageIndex)) pageIndex = 1;

        var shopItems = await _shopItemService.GetGameShopItemListAsync(pageIndex, _pageSize, 1, true);
        if (!shopItems.Any()) return NotFound();

        var shopItemsDto = _mapper.Map<List<ShopItemDtoToAdmin>>(shopItems);
        var model = new PaginatedItemsDtoModel<ShopItemDtoToAdmin>(pageIndex, _pageSize, shopItemsCount, shopItemsDto);
        return Ok(model);
    }

    /// <summary>
    /// 用户——获取商品售卖信息
    /// </summary>
    /// <param name="pageIndex">pageIndex=10</param>
    /// <param name="orderby">0-新品 1-折扣 2-热销</param>
    /// <returns></returns>
    [HttpGet("shop/items")]
    [AllowAnonymous]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<ShopItemDtoToUser>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetShopItemsToUserAsync([FromQuery] int pageIndex = 1, [FromQuery] int? orderby = 0)
    {
        var shopItemsCount = await _shopItemService.CountGameShopItemAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(shopItemsCount, _pageSize, pageIndex)) pageIndex = 1;
        if (!orderby.HasValue) orderby = 0;

        //获取商品信息
        var shopItems = await _shopItemService.GetGameShopItemListAsync(pageIndex, _pageSize, orderby.Value, false);
        if (!shopItems.Any()) return NotFound();

        //收集游戏信息Id
        var gameIds = shopItems.Select(shopItem => shopItem.GameInfoId).ToList();

        //调用Evaluation服务获取推荐测评文章
        using var response = await _evaluationService.GetShopArticlesAsync(gameIds);
        var articles = new List<ArticleShopDto>();
        if (response.IsSuccessStatusCode) articles = await response.Content.ReadFromJsonAsync<List<ArticleShopDto>>();

        var shopItemsDto = _mapper.Map<List<ShopItemDtoToUser>>(shopItems);
        var model = new PaginatedShopItemsDto<ShopItemDtoToUser>(pageIndex, _pageSize, shopItemsCount, shopItemsDto, articles);
        return Ok(model);
    }

    /// <summary>
    /// 用户——获取商品详细展示信息
    /// </summary>
    /// <param name="itemId">商品Id</param>
    /// <returns></returns>
    [HttpGet("shop/{itemId:int}", Name = nameof(GetShopItemsByIdAsync))]
    [AllowAnonymous]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ShopItemDetailDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetShopItemsByIdAsync([FromRoute] int itemId)
    {
        if (itemId <= 0 || itemId >= int.MaxValue) return BadRequest();

        var shopItem = await _shopItemService.GetGameShopItemByIdAsync(itemId);
        if (shopItem == null) return NotFound();

        var itemDetailToUser = _mapper.Map<ShopItemDetailDto>(shopItem);
        return Ok(itemDetailToUser);
    }

    /// <summary>
    /// 管理员——获取商品额外详细信息
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    [HttpGet("game/shop/{itemId:int}", Name = nameof(GetShopItemsByIdForAdminAsync))]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ShopItemDtoToAdmin), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetShopItemsByIdForAdminAsync([FromRoute] int itemId)
    {
        if (itemId <= 0 || itemId >= int.MaxValue) return BadRequest();

        var shopItem = await _shopItemService.GetGameShopItemByIdAsync(itemId);
        if (shopItem == null) return NotFound();

        var itemDtoToAdmin = _mapper.Map<ShopItemDtoToAdmin>(shopItem);
        return Ok(itemDtoToAdmin);
    }

    /// <summary>
    /// 用户，管理员——获取游戏已上架商品信息(也可用于判断该游戏是否有上架商品)
    /// </summary>
    /// <param name="gameId">游戏Id</param>
    /// <returns></returns>
    [HttpGet("shop", Name = nameof(CheckShopItemsByGameIdAsync))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ShopItemDtoToUser), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CheckShopItemsByGameIdAsync([FromQuery] int gameId)
    {
        if (gameId <= 0 || gameId >= int.MaxValue) return BadRequest();

        var shopItem = await _shopItemService.GetGameShopItemByGameIdAsync(gameId);
        if (shopItem == null) return NotFound();

        var itemDtoToUser = _mapper.Map<ShopItemDtoToUser>(shopItem);
        return Ok(itemDtoToUser);
    }

    /// <summary>
    /// 管理员——新增游戏商品信息
    /// </summary>
    /// <param name="addDto"></param>
    /// <returns></returns>
    [HttpPost("game/shop")] //定义该Action为HTTP POST
    [Authorize(Roles = "administrator")] //定义该方法需要身份验证且授权给administrator用户
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PublishShopItemAsync([FromBody] ShopItemAddDto addDto) //规定参数从HTTP Body中接受
    {
        //如果Request Body为空，则返回400
        if (addDto == null) return BadRequest();

        //映射DTO实体为Model实体
        var entityToAdd = _mapper.Map<GameShopItem>(addDto);

        if (await _shopItemService.HasSameGameShopAsync(addDto.GameInfoId))
            return BadRequest("该游戏的附属商品已经存在");

        //新增商品上架信息，并开启事务
        try
        {
            await _shopItemService.AddGameShopItemAsync(entityToAdd);
            var preResponse = await _unitOfWorkService.SaveEntitiesAsync();
            if (preResponse == false)
                throw new GameRepoDomainException("新增商品信息事务开启失败");

            // 批量生成游戏SDK
            await _sdkService.GenerateSDKForGameShopItemAsync(entityToAdd.AvailableStock, entityToAdd.Id);
            // Redis字段创建
            await _redisDatabase.Database.StringSetAsync($"ProductStock_{entityToAdd.Id}", entityToAdd.AvailableStock);
            // 事务提交
            var saveResponse = await _unitOfWorkService.SaveEntitiesAsync();
            if (saveResponse == false)
            {
                _logger.LogError("administrator: id:{UserId}, name:{UserName} add a shopItem error -> shopInfo:{@shop}",
                    User.FindFirst("sub").Value, User.FindFirst("nickname"), addDto);
                throw new GameRepoDomainException("新增商品信息事务失败");
            }

            _logger.LogInformation("administrator: id:{UserId}, name:{UserName} add a shopItem -> shopInfo:{@shop}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), addDto);
            return CreatedAtRoute(nameof(GetShopItemsByIdForAdminAsync), new { itemId = entityToAdd.Id }, new { itemId = entityToAdd.Id });
        }
        catch (MySqlException ex)
        {
            _logger.LogInformation("administrator: id:{UserId}, name:{UserName} add a shopItem occurred databaseError -> shopInfo:{@shop} | ErrorMessage:{Message}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), addDto, ex.Message);
            throw new GameRepoDomainException("数据库错误导致新增商品信息失败", ex.InnerException);
        }
    }

    /// <summary>
    /// 管理员——修改游戏商品信息
    /// </summary>
    /// <param name="updateDto">不允许使用该方法修改库存</param>
    /// <returns></returns>
    [HttpPut("game/shop")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateShopItemInfoAsync([FromBody] ShopItemUpdateDto updateDto)
    {
        var shopItemToUpdate = await _shopItemService.GetGameShopItemByIdAsync(updateDto.Id);
        if (shopItemToUpdate == null) return BadRequest();

        _mapper.Map(updateDto, shopItemToUpdate);
        var updateResponse = await _shopItemService.UpdateGameShopItemInfoAsync(shopItemToUpdate);
        if (updateResponse == false)
        {
            _logger.LogError("administrator: id:{UserId}, name:{UserName} update a shopItem error -> old:{@old} new:{@new}",
                User.FindFirst("sub").Value, User.FindFirst("nickname"), shopItemToUpdate, updateDto);
            throw new GameRepoDomainException("修改商品信息失败");
        }

        _logger.LogInformation("administrator: id:{UserId}, name:{UserName} update a shopItem -> old:{@old} new:{@new}",
            User.FindFirst("sub").Value, User.FindFirst("nickname"), shopItemToUpdate, updateDto);
        return NoContent();
    }

    /// <summary>
    /// 管理员——下架或上架游戏商品
    /// </summary>
    /// <param name="itemId">商品Id</param>
    /// <returns></returns>
    [HttpPut("game/shop/status/{itemId:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ChangeShopItemStatusAsync([FromRoute] int itemId)
    {
        //检查商品是否存在和商品状态
        var shopItem = await _shopItemService.CheckShopStatusAsync(itemId);
        //不存在该商品
        if (shopItem == null) return BadRequest();

        //Redis键不存在 程序错误
        if (await _redisDatabase.ExistsAsync(GetProductStockKey(itemId)) == false)
        {
            throw new GameRepoDomainException("分布式锁失效, 商品库存获取失败");
        }

        if (!shopItem.TemporaryStopSell.HasValue || shopItem.TemporaryStopSell.Value == false) //该商品已上架
        {
            _logger.LogInformation("admin:{name} take down shopItem:{id}, will sync stock in next step",
                User.FindFirstValue("nickname"), itemId);
            //从Redis中获取当前库存
            var currentShopStock = await _redisDatabase.GetAsync<int>(GetProductStockKey(itemId));

            //下架操作更新库存在更换状态
            await _shopItemService.UpdateShopItemStockWhenTakeDownAsync(itemId, currentShopStock);
            await _shopItemService.TakeDownGameShopItemAsync(itemId);
            var takeDownResponse = await _unitOfWorkService.SaveEntitiesAsync();

            //事务失败
            if (takeDownResponse == false)
            {
                _logger.LogError("admin:{name} take down shopItem:{id} error, database transaction rollback",
                    User.FindFirstValue("nickname"), itemId);
                throw new GameRepoDomainException("商品下架失败");
            }

            _logger.LogInformation("admin:{name} take down shopItem:{id} successfully", User.FindFirstValue("nickname"), itemId);
            return NoContent();
        }
        else //该商品处于下架状态
        {
            //进行商品上架操作
            //修改商品上架状态，并获取当前库存量进行分布式锁设置
            var shopStock = await _shopItemService.TakeUpGameShopItemAsync(itemId);
            var takeUpResponse = await _unitOfWorkService.SaveEntitiesAsync();
            if (takeUpResponse == false)
            {
                _logger.LogError("admin:{name} take up shopItem:{id} error, database transaction rollback",
                    User.FindFirstValue("nickname"), itemId);
                throw new GameRepoDomainException("商品上架失败");
            }

            await using var redLock = await _distributedLockFactory.CreateLockAsync(
                GetProductStockKey(itemId),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(1)
            );
            if (redLock.IsAcquired)
            {
                //商品上架状态修改成功 开始上锁
                var redisAdd = await _redisDatabase.Database.StringSetAsync(GetProductStockKey(itemId), shopStock);
                if (redisAdd == false)
                {
                    _logger.LogError("admin:{name} take up shopItem:{id}, sync stock:{StockKey} add fail",
                        User.FindFirstValue("nickname"), itemId, GetProductStockKey(itemId));
                    throw new GameRepoDomainException("商品上架成功,但分布式锁设置失败");
                }
            }
            else
            {
                _logger.LogError("admin:{name} take up shopItem:{id}, get sync stock:{StockKey} fail",
                    User.FindFirstValue("nickname"), itemId, GetProductStockKey(itemId));
                throw new GameRepoDomainException("获取分布式锁失败");
            }
            _logger.LogInformation("admin:{name} take up shopItem:{id} successfully", User.FindFirstValue("nickname"), itemId);
            return NoContent();
        }
    }

    /// <summary>
    /// 管理员——修改商品库存(商品处于下架状态才允许修改库存)
    /// </summary>
    /// <param name="stockUpdateDto"></param>
    /// <returns></returns>
    [HttpPut("game/shop/stock")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ChangeShopItemStockAsync([FromBody] ShopItemStockUpdateDto stockUpdateDto)
    {
        //检查商品是否满足修改库存条件
        var shopStatusCheck = await _shopItemService.CheckShopStatusAsync(stockUpdateDto.Id);
        if (shopStatusCheck == null || shopStatusCheck.TemporaryStopSell == false) return BadRequest("商品不存在，或商品还未下架");

        //Redis键不存在 程序错误
        if (await _redisDatabase.ExistsAsync(GetProductStockKey(stockUpdateDto.Id)) == false)
        {
            throw new GameRepoDomainException("分布式锁失效, 商品库存无法更新");
        }
        var shopItem = await _shopItemService.GetGameShopItemByIdAsync(stockUpdateDto.Id);
        _logger.LogInformation("admin:{name} wanna change shopItem:{id} stock", User.FindFirstValue("nickname"), stockUpdateDto.Id);

        // 增加库存
        if (shopItem.AvailableStock < stockUpdateDto.AvailableStock)
        {
            _logger.LogInformation("now shopItem:{id} stock is {now}, add to {new}", stockUpdateDto.Id, shopItem.AvailableStock, stockUpdateDto.AvailableStock);
            var generateCount = stockUpdateDto.AvailableStock - shopItem.AvailableStock;
            await _shopItemService.UpdateShopItemStockWhenChangeNumberAsync(stockUpdateDto.Id, stockUpdateDto.AvailableStock);
            await _sdkService.GenerateSDKForGameShopItemAsync(generateCount, stockUpdateDto.Id);
            var addResponse = await _unitOfWorkService.SaveChangesAsync();
            if (addResponse < generateCount)
            {
                _logger.LogInformation("shopItem:{id} stock is {now}, add to {new} fail, data will roll back",
                    stockUpdateDto.Id, shopItem.AvailableStock, stockUpdateDto.AvailableStock);
                throw new GameRepoDomainException("新增商品库存事务失败");
            }
        }
        else if (shopItem.AvailableStock > stockUpdateDto.AvailableStock) //减少库存
        {
            _logger.LogInformation("now shopItem:{id} stock is {now}, reduce to {new}", stockUpdateDto.Id, shopItem.AvailableStock, stockUpdateDto.AvailableStock);
            var deleteCount = shopItem.AvailableStock - stockUpdateDto.AvailableStock;
            await _shopItemService.UpdateShopItemStockWhenChangeNumberAsync(stockUpdateDto.Id, stockUpdateDto.AvailableStock);
            //删除未发出的SDK
            await _sdkService.BatchDeleteGameItemsSDKAsync(stockUpdateDto.Id, null, deleteCount);
            var reduceResponse = await _unitOfWorkService.SaveChangesAsync();
            if (reduceResponse < deleteCount)
            {
                _logger.LogInformation("shopItem:{id} stock is {now}, reduce to {new} fail, data will roll back",
                    stockUpdateDto.Id, shopItem.AvailableStock, stockUpdateDto.AvailableStock);
                throw new GameRepoDomainException("减少商品库存事务失败");
            }
        }

        await using var redLock = await _distributedLockFactory.CreateLockAsync(
            GetProductStockKey(stockUpdateDto.Id),
            TimeSpan.FromSeconds(30),
            TimeSpan.FromSeconds(20),
            TimeSpan.FromSeconds(1)
        );
        if (redLock.IsAcquired)
        {
            var redisAdd = await _redisDatabase.Database.StringSetAsync(GetProductStockKey(stockUpdateDto.Id), stockUpdateDto.AvailableStock);
            if (redisAdd == false)
            {
                _logger.LogError("admin:{name} change shopItem:{id} stock, sync stock:{StockKey} add fail",
                    User.FindFirstValue("nickname"), stockUpdateDto.Id, GetProductStockKey(stockUpdateDto.Id));
                throw new GameRepoDomainException("商品库存修改成功,但分布式锁设置失败");
            }
        }
        else
        {
            _logger.LogError("admin:{name} change shopItem:{id} stock, get sync stock:{StockKey} fail",
                User.FindFirstValue("nickname"), stockUpdateDto.Id, GetProductStockKey(stockUpdateDto.Id));
            throw new GameRepoDomainException("获取分布式锁失败");
        }


        return NoContent();
    }

    /// <summary>
    /// 管理员——删除商品信息(已上架开售的商品不能进行删除)
    /// </summary>
    /// <param name="itemId">商品Id</param>
    /// <returns></returns>
    [HttpDelete("game/shop/{itemId:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteShopItemAsync([FromRoute] int itemId)
    {
        var checkShopStatus = await _shopItemService.CheckShopStatusAsync(itemId);
        if (checkShopStatus == null || checkShopStatus.TemporaryStopSell == false) return BadRequest("商品不存在或商品已上架"); // 商品不存在或商品已上架

        //Redis键不存在 程序错误
        if (await _redisDatabase.ExistsAsync(GetProductStockKey(itemId)) == false)
        {
            throw new GameRepoDomainException("分布式锁失效, 商品库存无法更新");
        }

        //商品下架进行删除前，判断该商品是否已经发放过SDK
        var removeShopItem = await _shopItemService.GetGameShopItemByIdAsync(itemId);
        var availableStock = await _sdkService.CountSDKNumberByGameItemOrStatusAsync(removeShopItem.Id, null);
        if (availableStock != removeShopItem.AvailableStock) return BadRequest("该商品已出售部分，不允许被删除");

        //删除商品和SDK信息
        await _sdkService.BatchDeleteGameItemsSDKAsync(removeShopItem.Id, null, (int)availableStock);
        var preResponse = await _unitOfWorkService.SaveEntitiesAsync();
        if (preResponse == false)
        {
            throw new GameRepoDomainException("删除商品事务发起失败");
        }
        await _shopItemService.DeleteGameShopItemByIdAsync(itemId);
        var removeResponse = await _unitOfWorkService.SaveEntitiesAsync();
        if (removeResponse == false)
        {
            _logger.LogError("shopItem:{id} stock is {now}, delete this shop but fail, data will roll back", removeShopItem.Id, availableStock);
            throw new GameRepoDomainException("删除商品事务失败");
        }

        //获取分布式锁
        await using var redLock = await _distributedLockFactory.CreateLockAsync(
            GetProductStockKey(itemId),
            TimeSpan.FromSeconds(30),
            TimeSpan.FromSeconds(20),
            TimeSpan.FromSeconds(1)
        );
        if (redLock.IsAcquired)
        {
            //移除分布式锁
            var redisRemove = await _redisDatabase.RemoveAsync(GetProductStockKey(itemId));
            if (redisRemove == false)
            {
                _logger.LogError("admin:{name} take down shopItem:{id}, sync stock:{StockKey} delete fail",
                    User.FindFirstValue("nickname"), itemId, GetProductStockKey(itemId));
                throw new GameRepoDomainException("商品下架成功,但分布式锁移除失败");
            }
        }
        else
        {
            _logger.LogError("admin:{name} take down shopItem:{id}, get sync stock:{StockKey} fail",
                User.FindFirstValue("nickname"), itemId, GetProductStockKey(itemId));
            throw new GameRepoDomainException("获取分布式锁失败");
        }
        return NoContent();
    }

    [NonAction]
    private string GetProductStockKey(int productId) => $"ProductStock_{productId}";
}