namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.Controllers;

[ApiController]
[Route("api/v1/order")]
public class PurchaseController : ControllerBase
{
    private readonly ILogger<PurchaseController> _logger;
    private readonly IRedisDatabase _redisDatabase;
    private readonly IDistributedLockFactory _distributedLockFactory;
    private readonly RepoCallService _repoCallService;
    private readonly GrpcRepoCallService _grpcRepoCallService;

    public PurchaseController(
        ILogger<PurchaseController> logger,
        IRedisClient redisClient,
        IRedisDatabase redisDatabase,
        IDistributedLockFactory distributedLockFactory,
        RepoCallService repoCallService,
        GrpcRepoCallService grpcRepoCallService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
        _distributedLockFactory = distributedLockFactory ?? throw new ArgumentNullException(nameof(distributedLockFactory));
        _repoCallService = repoCallService ?? throw new ArgumentNullException(nameof(repoCallService));
        _grpcRepoCallService = grpcRepoCallService ?? throw new ArgumentNullException(nameof(grpcRepoCallService));
    }

    [HttpPost("item/{shopItemId:int}")]
    [Authorize]
    public async Task<IActionResult> BuyShopItemAsync([FromRoute] int shopItemId)
    {
        var productKey = GetProductStockKey(shopItemId);

        //没有此商品
        if (!_redisDatabase.Database.KeyExists(productKey)) return BadRequest();

        //有此商品 上锁进行库存扣除
        //锁的过期时间为30s，等待获取锁的时间为20s，如果没有获取到锁，则等待1秒钟后再次尝试获取
        await using var redLock = await _distributedLockFactory.CreateLockAsync(
            resource: shopItemId.ToString(),
            expiryTime: TimeSpan.FromSeconds(30),
            waitTime: TimeSpan.FromSeconds(20),
            retryTime: TimeSpan.FromSeconds(1)
        );

        // 确认是否已获取到锁
        if (redLock.IsAcquired)
        {
            _logger.LogInformation("shopItemId:{id} get the distribute locked", shopItemId);
            var stockKey = GetProductStockKey(shopItemId);
            var currentQuantity = (int)await _redisDatabase.Database.StringGetAsync(stockKey);

            if (currentQuantity < 1)
            {
                _logger.LogInformation("shopItem:{id} all sell, begin grpc call gameRepo to stop this", shopItemId);
                var response = await _grpcRepoCallService.StopShopSellAsync(shopItemId);
                if (response == false)
                    _logger.LogError("shopItem:{id} all sell, begin grpc call gameRepo to stop this but fail", shopItemId);
                return BadRequest();
            }

            await _redisDatabase.Database.StringDecrementAsync(stockKey, 1);
            //通信服务 发送SDK保存拥有记录
            await _repoCallService.SaveBuyerRecordAsync(User.FindFirstValue("sub"), shopItemId);
            _logger.LogInformation("user:{name} buy a shopItem:{id}", User.FindFirstValue("nickname"), shopItemId);
        }
        else
        {
            _logger.LogWarning("when user wanna buy a item:{itemId}, but get lock fail", shopItemId);
            throw new OrderingDomainException("user wanna buy a item but get lock fail");
        }

        return Ok();
    }


    [NonAction]
    private static string GetProductStockKey(int productId) => $"ProductStock_{productId}";
}
