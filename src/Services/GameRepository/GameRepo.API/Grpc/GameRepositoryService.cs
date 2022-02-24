namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Grpc;

public class GameRepositoryService : GameRepository.GameRepositoryBase
{
    private readonly ILogger<GameRepositoryService> _logger;
    private readonly IGameShopItemService _shopItemService;

    public GameRepositoryService(IGameShopItemService shopItemService, ILogger<GameRepositoryService> logger)
    {
        _shopItemService = shopItemService ?? throw new ArgumentNullException(nameof(shopItemService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<shopStatusChangeResponse> ChangeShopSellStatus(shopStatusChangeRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Begin grpc call from method {Method} for shopItem id {Id}", context.Method,
            request.ShopItemId);

        await _shopItemService.UpdateShopItemStockWhenTakeDownAsync(request.ShopItemId);
        var response = await _shopItemService.ChangeGameShopItemStatusAsync(request.ShopItemId);
        if (response == true)
        {
            context.Status = new Status(StatusCode.OK, $"shopItem with id {request.ShopItemId} has been stop sell");
            _logger.LogInformation("call via grpc: status:{status}", context.Status);

            var result = new shopStatusChangeResponse
            {
                ShopItemId = request.ShopItemId,
                StopSell = true
            };
            return result;
        }
        else
        {
            context.Status = new Status(StatusCode.Internal,
                $"shopItem with id {request.ShopItemId} has stop sell fail");
            _logger.LogInformation("call via grpc: status:{status}", context.Status);
            var result = new shopStatusChangeResponse
            {
                ShopItemId = request.ShopItemId,
                StopSell = false
            };
            return result;
        }
    }
}