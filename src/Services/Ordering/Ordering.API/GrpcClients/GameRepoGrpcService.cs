namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.GrpcClients;

public class GameRepoGrpcService
{
    private readonly GameRepository.GameRepositoryClient _client;
    private readonly ILogger<GameRepoGrpcService> _logger;

    public GameRepoGrpcService(GameRepository.GameRepositoryClient client, ILogger<GameRepoGrpcService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> StopShopSellAsync(int shopItemId)
    {
        var request = new shopStatusChangeRequest
        {
            ShopItemId = shopItemId
        };
        _logger.LogInformation("grpc request {@request}", request);
        var response = await _client.ChangeShopSellStatusAsync(request, deadline: DateTime.UtcNow.AddSeconds(3));
        _logger.LogInformation("grpc response {@response}", response);
        return true;
    }
}