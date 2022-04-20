using GrpcGameRepository;

namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.GrpcClients;

public class GameRepoGrpcService
{
    private readonly GameRepository.GameRepositoryClient _client;
    private readonly ILogger<GameRepoGrpcService> _logger;

    public GameRepoGrpcService(GameRepository.GameRepositoryClient client, ILogger<GameRepoGrpcService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<gameStatisticsResponse> GetGameStatisticsAsync()
    {
        var request = new gameStatisticsRequest();
        return await _client.GetGameStatisticsInfoAsync(request);
    }
}