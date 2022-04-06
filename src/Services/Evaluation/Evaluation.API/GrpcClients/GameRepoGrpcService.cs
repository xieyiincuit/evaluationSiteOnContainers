namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.GrpcClients;

public class GameRepoGrpcService
{
    private readonly GameRepository.GameRepositoryClient _client;
    private readonly ILogger<GameRepoGrpcService> _logger;

    public GameRepoGrpcService(GameRepository.GameRepositoryClient client, ILogger<GameRepoGrpcService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<gameInfoResponse> GetGameInfoAsync(int gameId)
    {
        var request = new gameInfoRequest
        {
            GameId = gameId
        };
        _logger.LogInformation("grpc request {@request}", request);
        var response = await _client.GetGameInformationAsync(request, deadline: DateTime.UtcNow.AddSeconds(3));
        _logger.LogInformation("grpc response {@response}", response);
        return response;
    }

    public async Task<bool> CheckGameExistAsync(int gameId)
    {
        var request = new gameInfoRequest
        {
            GameId = gameId
        };
        _logger.LogInformation("grpc request {@request}", request);
        var response = await _client.GetGameInformationAsync(request, deadline: DateTime.UtcNow.AddSeconds(3));
        _logger.LogInformation("grpc response {@response}", response);
        return response.GameId != 0;
    }
}