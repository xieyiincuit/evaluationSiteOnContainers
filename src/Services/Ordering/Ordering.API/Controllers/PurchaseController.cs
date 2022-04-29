using Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.IntegrationEvents.Events;

namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.Controllers;

/// <summary>
/// 游戏订购接口
/// </summary>
[ApiController]
[Route("api/v1/order")]
public class PurchaseController : ControllerBase
{
    private readonly IDistributedLockFactory _distributedLockFactory;
    private readonly IOrderIntegrationEventService _orderIntegrationEventService;
    private readonly GameRepoGrpcService _gameRepoGrpcService;
    private readonly ILogger<PurchaseController> _logger;
    private readonly IRedisDatabase _redisDatabase;
    private readonly GameRepoHttpClient _gameRepoHttpClient;

    public PurchaseController(
        ILogger<PurchaseController> logger,
        IRedisDatabase redisDatabase,
        IDistributedLockFactory distributedLockFactory,
        IOrderIntegrationEventService orderIntegrationEventService,
        GameRepoHttpClient gameRepoHttpClient,
        GameRepoGrpcService gameRepoGrpcService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
        _distributedLockFactory = distributedLockFactory ?? throw new ArgumentNullException(nameof(distributedLockFactory));
        _orderIntegrationEventService = orderIntegrationEventService ?? throw new ArgumentNullException(nameof(orderIntegrationEventService));
        _gameRepoHttpClient = gameRepoHttpClient ?? throw new ArgumentNullException(nameof(gameRepoHttpClient));
        _gameRepoGrpcService = gameRepoGrpcService ?? throw new ArgumentNullException(nameof(gameRepoGrpcService));
    }

    /// <summary>
    /// 用户——购买游戏商品
    /// </summary>
    /// <param name="shopItemId"></param>
    /// <returns></returns>
    /// <exception cref="OrderingDomainException"></exception>
    [HttpPost("item/{shopItemId:int}")]
    [Authorize]
    public async Task<IActionResult> BuyShopItemAsync([FromRoute] int shopItemId)
    {
        // 生成商品Key 用于做分布式锁
        var productKey = GetProductStockKey(shopItemId);

        // 若用户请求的商品不存在 返回400
        if (!_redisDatabase.Database.KeyExists(productKey) || (int)await _redisDatabase.Database.StringGetAsync(productKey) < 1)
            return BadRequest();

        //有此商品 获取分布式锁进行库存扣除
        //锁的过期时间为30s，等待获取锁的时间为20s，如果没有获取到锁，则等待1秒钟后再次尝试获取
        await using var redLock = await _distributedLockFactory.CreateLockAsync(
            shopItemId.ToString(),
            TimeSpan.FromSeconds(30),
            TimeSpan.FromSeconds(20),
            TimeSpan.FromSeconds(1)
        );

        // 确认是否已获取到锁
        if (redLock.IsAcquired)
        {
            _logger.LogInformation("shopItemId:{id} get the distribute locked", shopItemId);

            // 通信服务 发送SDK保存拥有记录
            var sdkSendResponse = await _gameRepoHttpClient.SaveBuyerRecordAsync(User.FindFirstValue("sub"), shopItemId);
            if (!sdkSendResponse.IsSuccessStatusCode)
            {
                _logger.LogError("user:{name} buy a shopItem:{id} error", User.FindFirstValue("nickname"), shopItemId);
                throw new OrderingDomainException("商品购买时SDK发放失败");
            }

            // 异步更改游戏热度
            var userBuyEvent = new BuyGameIntegrationEvent(shopItemId);
            await _orderIntegrationEventService.PublishThroughEventBusAsync(userBuyEvent);
            // 商品库存减少
            await _redisDatabase.Database.StringDecrementAsync(productKey, 1);
            // 获取当前商品库存
            var currentQuantity = (int)await _redisDatabase.Database.StringGetAsync(productKey);
            // 该商品售罄
            if (currentQuantity < 1)
            {
                _logger.LogInformation("shopItem:{id} all sell, begin grpc call gameRepo to stop this", shopItemId);
                //GRPC通知游戏资料服务下架该商品
                var response = await _gameRepoGrpcService.StopShopSellAsync(shopItemId);
                if (response == false)
                {
                    _logger.LogError("shopItem:{id} all sell, begin grpc call gameRepo to stop this but fail", shopItemId);
                    throw new OrderingDomainException("商品售罄，但下架商品失败");
                }
            }
            _logger.LogInformation("user:{name} buy a shopItem:{id}", User.FindFirstValue("nickname"), shopItemId);
        }
        else
        {
            _logger.LogWarning("when user wanna buy a item:{itemId}, but get lock fail", shopItemId);
            throw new OrderingDomainException($"user:{User.FindFirstValue("nickname")} wanna buy a item but get lock fail");
        }

        return Ok();
    }


    [NonAction]
    private static string GetProductStockKey(int productId)
    {
        return $"ProductStock_{productId}";
    }
}