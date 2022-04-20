using GrpcEvaluation;

namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.GrpcClients;

public class EvaluationRepoGrpcService
{
    private readonly EvaluationRepository.EvaluationRepositoryClient _client;
    private readonly ILogger<EvaluationRepoGrpcService> _logger;

    public EvaluationRepoGrpcService(EvaluationRepository.EvaluationRepositoryClient client, ILogger<EvaluationRepoGrpcService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<articleStatisticsResponse> GetArticleStatisticsAsync()
    {
        var request = new articleStatisticsRequest();
        return await _client.GetArticleStatisticsInfoAsync(request);
    }
}