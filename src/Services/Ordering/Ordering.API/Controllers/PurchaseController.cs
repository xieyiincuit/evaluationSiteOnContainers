namespace Ordering.API.Controllers;

[ApiController]
[Route("api/v1/o")]
public class PurchaseController : ControllerBase
{
    private readonly ILogger<PurchaseController> _logger;
    private readonly IRedisClient _redisClient;
    private readonly IRedisDatabase _redisDatabase;
    private readonly IDistributedLockFactory _distributedLockFactory;

    public PurchaseController(
        ILogger<PurchaseController> logger,
        IRedisClient redisClient,
        IRedisDatabase redisDatabase,
        IDistributedLockFactory distributedLockFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
        _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
        _distributedLockFactory = distributedLockFactory ?? throw new ArgumentNullException(nameof(distributedLockFactory));
    }

    [HttpPost("item")]
    [Authorize]
    public async Task<IActionResult> BuyShopItemAsync(int shopItemId)
    {
        await _redisDatabase.Database.StringSetAsync("name", "zhousl");
        return NoContent();
    }

}
