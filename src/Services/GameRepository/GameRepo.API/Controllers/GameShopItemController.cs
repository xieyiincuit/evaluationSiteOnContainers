namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1")]
public class GameShopItemController : ControllerBase
{
    private const int _pageSize = 10;
    private readonly ILogger<GameShopItemController> _logger;
    private readonly IMapper _mapper;
    private readonly IRedisDatabase _redisDatabase;
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
        EvaluationCallService evaluationService)
    {
        _shopItemService = shopItemService ?? throw new ArgumentNullException(nameof(shopItemService));
        _sdkService = sdkService ?? throw new ArgumentNullException(nameof(sdkService));
        _unitOfWorkService = unitOfWorkService ?? throw new ArgumentNullException(nameof(unitOfWorkService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
        _evaluationService = evaluationService ?? throw new ArgumentNullException(nameof(evaluationService));
    }

    /// <summary>
    /// 获取商品信息To管理员
    /// </summary>
    /// <param name="pageIndex">分页大小定死为10</param>
    /// <returns></returns>
    [HttpGet]
    [Route("game/shops")]
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

    [HttpGet]
    [Route("shop/items")]
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

        var gameIds = new List<int>();
        foreach (var shopItem in shopItems)
        {
            gameIds.Add(shopItem.GameInfoId);
        }

        using var response = await _evaluationService.GetShopArticlesAsync(gameIds);
        var articles = new List<ArticleShopDto>();
        if (response.IsSuccessStatusCode) articles = await response.Content.ReadFromJsonAsync<List<ArticleShopDto>>();

        var shopItemsDto = _mapper.Map<List<ShopItemDtoToUser>>(shopItems);
        var model = new PaginatedShopItemsDto<ShopItemDtoToUser>(pageIndex, _pageSize, shopItemsCount, shopItemsDto, articles);
        return Ok(model);
    }

    [HttpGet("shop/{itemId:int}", Name = nameof(GetShopItemsByIdAsync))]
    [AllowAnonymous]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ShopItemDtoToUser), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetShopItemsByIdAsync([FromRoute] int itemId)
    {
        if (itemId <= 0 || itemId >= int.MaxValue) return BadRequest();

        var shopItem = await _shopItemService.GetGameShopItemByIdAsync(itemId);
        if (shopItem == null) return NotFound();

        var itemDetailToUser = _mapper.Map<ShopItemDetailDto>(shopItem);
        return Ok(itemDetailToUser);
    }

    /// <summary>
    /// 管理员获取特定商品信息
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

    [HttpGet("shop", Name = nameof(GetShopItemsByGameIdAsync))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ShopItemDtoToUser), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetShopItemsByGameIdAsync([FromQuery] int gameId)
    {
        if (gameId <= 0 || gameId >= int.MaxValue) return BadRequest();

        var shopItem = await _shopItemService.GetGameShopItemByGameIdAsync(gameId);
        if (shopItem == null) return NotFound();

        var itemDtoToUser = _mapper.Map<ShopItemDtoToUser>(shopItem);
        return Ok(itemDtoToUser);
    }

    /// <summary>
    /// 新增商品信息
    /// </summary>
    /// <param name="addDto"></param>
    /// <returns></returns>
    [HttpPost] //定义该Action为HTTP POST
    [Route("game/shop")] //定义子路由
    [Authorize(Roles = "administrator")] //定义该方法需要身份验证且授权给administrator用户
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PublishShopItemAsync([FromBody] ShopItemAddDto addDto) //规定参数从HTTP Body中接受
    {
        //如果Request Body为空，则返回400
        if (addDto == null) return BadRequest();

        //映射DTO实体为Model实体
        var entityToAdd = _mapper.Map<GameShopItem>(addDto);

        _logger.LogInformation($"administrator: id:{User.FindFirst("sub").Value}, name:{User.Identity.Name} add a shopItem");

        //新增商品上架信息，并开启事务
        var firstCreated = await _shopItemService.AddGameShopItemAsync(entityToAdd);
        //事务开启失败，也即是添加失败，返回错误。
        if (firstCreated != true) return BadRequest();

        //批量生成SDK
        var response = await _sdkService.GenerateSDKForGameShopItemAsync(entityToAdd.AvailableStock, entityToAdd.Id);
        //Redis字段创建
        await _redisDatabase.Database.StringSetAsync($"ProductStock_{entityToAdd.Id}", entityToAdd.AvailableStock);

        if (response == true)
            return CreatedAtRoute(nameof(GetShopItemsByIdForAdminAsync), new { itemId = entityToAdd.Id }, null);
        return BadRequest();
    }

    /// <summary>
    /// 修改商品信息
    /// </summary>
    /// <param name="updateDto">不允许使用该方法修改库存</param>
    /// <returns></returns>
    [HttpPut]
    [Route("game/shop")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateShopItemInfoAsync([FromBody] ShopItemUpdateDto updateDto)
    {
        var shopItemToUpdate = await _shopItemService.GetGameShopItemByIdAsync(updateDto.Id);
        if (shopItemToUpdate == null) return BadRequest();

        _mapper.Map(updateDto, shopItemToUpdate);
        var response = await _shopItemService.UpdateGameShopItemInfoAsync(shopItemToUpdate);
        return response == true ? NoContent() : BadRequest();
    }

    /// <summary>
    /// 下架商品
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("game/shop/status/{itemId:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ChangeShopItemStatusAsync([FromRoute] int itemId)
    {
        _logger.LogInformation("admin:{name} take down shopItem:{id}, will sync stock in next step",
            User.FindFirstValue("nickname"), itemId);

        //先更新库存在更换状态
        var response = await _shopItemService.UpdateShopItemStockWhenTakeDownAsync(itemId);
        if (response == true)
        {
            await _shopItemService.ChangeGameShopItemStatusAsync(itemId);
            _logger.LogInformation("admin:{name} take down shopItem:{id} successfully", User.FindFirstValue("nickname"),
                itemId);
        }

        return response == true ? NoContent() : BadRequest();
    }

    /// <summary>
    /// 修改商品库存
    /// </summary>
    /// <param name="stockUpdateDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("game/shop/stock")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ChangeShopItemStockAsync([FromBody] ShopItemStockUpdateDto stockUpdateDto)
    {
        var shopItem = await _shopItemService.GetGameShopItemByIdAsync(stockUpdateDto.Id);
        if (shopItem == null) return BadRequest();

        _logger.LogInformation("admin:{name} wanna change shopItem:{id} stock", User.FindFirstValue("nickname"),
            stockUpdateDto.Id);

        if (shopItem.AvailableStock < stockUpdateDto.AvailableStock)
        {
            _logger.LogInformation("now shopItem:{id} stock is {now}, add to {new}", stockUpdateDto.Id,
                shopItem.AvailableStock, stockUpdateDto.AvailableStock);
            var generateCount = stockUpdateDto.AvailableStock - shopItem.AvailableStock;
            await _shopItemService.UpdateShopItemStockWhenChangeNumberAsync(stockUpdateDto.Id,
                stockUpdateDto.AvailableStock);
            await _sdkService.GenerateSDKForGameShopItemAsync(generateCount, stockUpdateDto.Id);
        }
        else if (shopItem.AvailableStock > stockUpdateDto.AvailableStock)
        {
            _logger.LogInformation("now shopItem:{id} stock is {now}, reduce to {new}", stockUpdateDto.Id,
                shopItem.AvailableStock, stockUpdateDto.AvailableStock);
            var deleteCount = shopItem.AvailableStock - shopItem.AvailableStock;
            await _shopItemService.UpdateShopItemStockWhenChangeNumberAsync(stockUpdateDto.Id,
                stockUpdateDto.AvailableStock);
            await _sdkService.BatchDeleteGameItemsSDKAsync(stockUpdateDto.Id, null, deleteCount);
        }

        return NoContent();
    }


    [HttpDelete]
    [Route("game/shop/{itemId:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteShopItemAsync([FromRoute] int itemId)
    {
        throw new NotImplementedException();
    }
}