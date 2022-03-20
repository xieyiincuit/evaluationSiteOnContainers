namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.HttpClients;

public class IdentityCallService
{
    private readonly HttpClient _client;
    private readonly ILogger<IdentityCallService> _logger;

    public IdentityCallService(HttpClient client, ILogger<IdentityCallService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HttpResponseMessage> GetCommentsUserProfileAsync(List<string> userIds)
    {
        _logger.LogDebug("---- Evaluation client call identity services: baseUrl:{url}", _client.BaseAddress);
        var callUrl = _client.BaseAddress + "api/v1/user/avatar/batch";

        try
        {
            //这里不需要身份认证
            //var header = _httpContextAccessor.HttpContext.Request.Headers;
            //_client.DefaultRequestHeaders.Add("Authorization", "Bearer" + header["Authorization"]);

            var postBody = JsonContent.Create(userIds, MediaTypeHeaderValue.Parse("application/json"));

            //这里不要dispose该对象，不然任务完成回调后不能访问到response了
            var response = await _client.PostAsync(callUrl, postBody);

            if (response is null)
            {
                _logger.LogError("http response return null when callTo:{callUrl}, bodyInfo:{@ids}", callUrl, userIds);
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