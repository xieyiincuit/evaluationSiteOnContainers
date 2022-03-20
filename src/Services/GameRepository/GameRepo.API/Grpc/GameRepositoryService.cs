namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Grpc;

public class GameRepositoryService : GameRepository.GameRepositoryBase
{
    private readonly ILogger<GameRepositoryService> _logger;
    private readonly IGameShopItemService _shopItemService;
    private readonly IGameInfoService _gameInfoService;


    public GameRepositoryService(IGameShopItemService shopItemService,
        IGameInfoService gameInfoService,
        ILogger<GameRepositoryService> logger)
    {
        _shopItemService = shopItemService ?? throw new ArgumentNullException(nameof(shopItemService));
        _gameInfoService = gameInfoService ?? throw new ArgumentNullException(nameof(gameInfoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<shopStatusChangeResponse> ChangeShopSellStatus(shopStatusChangeRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Begin grpc call from method {Method} for shopItem id {Id}", context.Method, request.ShopItemId);

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

    public override async Task<gameInfoResponse> GetGameInformation(gameInfoRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Begin grpc call from method {Method} for gameItem id {Id}", context.Method, request.GameId);
        var gameInfo = await _gameInfoService.GetGameInfoAsync(request.GameId);
        if (gameInfo == null)
        {
            context.Status = new Status(StatusCode.Internal, $"gameItem with id {request.GameId} is not exist");
            _logger.LogInformation("call via grpc: status:{status}", context.Status);

            var badResult = new gameInfoResponse { GameId = 0 };
            return badResult;
        }

        context.Status = new Status(StatusCode.OK, $"gameInfo with id {request.GameId} has been return");
        _logger.LogInformation("call via grpc: status:{status}", context.Status);

        var result = new gameInfoResponse
        {
            GameId = gameInfo.Id,
            GameName = gameInfo.Name,
            DescriptionPic = gameInfo.RoughPicture
        };
        return result;
    }
}