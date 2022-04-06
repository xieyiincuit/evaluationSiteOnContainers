using System.Net.Http.Headers;

namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.HttpClients;

public class EvaluationCallService
{
    private readonly HttpClient _client;
    private readonly ILogger<EvaluationCallService> _logger;

    public EvaluationCallService(HttpClient client, ILogger<EvaluationCallService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HttpResponseMessage> GetShopArticlesAsync(List<int> gameIds)
    {
        _logger.LogDebug("---- GameRepo client call evaluation services: baseUrl:{url}", _client.BaseAddress);
        var callUrl = _client.BaseAddress + "api/v1/shop/articles";

        try
        {
            var postBody = JsonContent.Create(gameIds, MediaTypeHeaderValue.Parse("application/json"));

            var response = await _client.PostAsync(callUrl, postBody);

            if (response is null)
            {
                _logger.LogError("http response return null when callTo:{callUrl}, bodyInfo:{@ids}", callUrl, gameIds);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            _logger.LogInformation($"received callback response -> callUrl:{callUrl}, statusCode:{response.StatusCode}");
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError($"httpClient to callback {callUrl} occurred an error -> message:{e.Message}");
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}
