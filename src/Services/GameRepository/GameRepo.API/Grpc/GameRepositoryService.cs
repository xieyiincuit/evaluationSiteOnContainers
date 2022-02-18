namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Grpc;

public class GameRepositoryService : GameRepository.GameRepositoryBase
{
    private readonly IGameShopItemService _shopItemService;
    private readonly ILogger<GameRepositoryService> _logger;

    public GameRepositoryService(IGameShopItemService shopItemService, ILogger<GameRepositoryService> logger)
    {
        _shopItemService = shopItemService ?? throw new ArgumentNullException(nameof(shopItemService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<shopStatusChangeResponse> ChangeShopSellStatus(shopStatusChangeRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Begin grpc call from method {Method} for shopItem id {Id}", context.Method, request.ShopItemId);

        await _shopItemService.UpdateShopItemStockAsync(request.ShopItemId);
        var response = await _shopItemService.ChangeGameShopItemStatusAsync(request.ShopItemId);
        if (response != false)
        {
            context.Status = new Status(StatusCode.OK, $"shopItem with id {request.ShopItemId} has been stop sell");
            return new shopStatusChangeResponse()
            {
                ShopItemId = request.ShopItemId,
                StopSell = true
            };
        }
        else
        {
            context.Status = new Status(StatusCode.Internal, $"shopItem with id {request.ShopItemId} has stop sell fail");
            return new shopStatusChangeResponse()
            {
                ShopItemId = request.ShopItemId,
                StopSell = false
            };
        }
    }
}