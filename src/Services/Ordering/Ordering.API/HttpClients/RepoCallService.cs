namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.HttpClients;

public class RepoCallService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<RepoCallService> _logger;

    public RepoCallService(HttpClient httpClient, ILogger<RepoCallService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<HttpResponseMessage> SaveBuyerRecordAsync(string userId, int shopItemId)
    {
        _logger.LogDebug("---- client call gamerepo services: baseUrl:{url}", _httpClient.BaseAddress);
        var callUrl = _httpClient.BaseAddress + "api/v1/user/sdk/send";

        try
        {
            var header = _httpContextAccessor.HttpContext.Request.Headers;
            _httpClient.DefaultRequestHeaders.Add("Authorization", header["Authorization"].ToString());
            var postBody = JsonContent.Create(new {ShopItemId = shopItemId, UserId = userId},
                MediaTypeHeaderValue.Parse("application/json"));

            var response = await _httpClient.PostAsync(callUrl, postBody);

            if (response is null)
            {
                _logger.LogError("http response return null when callTo:{callUrl}", callUrl);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            _logger.LogInformation(
                $"received callback response -> callUrl:{callUrl}, statusCode:{response.StatusCode}");
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError($"httpClient to callback {callUrl} occurred an error -> message:{e.Message}");
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}