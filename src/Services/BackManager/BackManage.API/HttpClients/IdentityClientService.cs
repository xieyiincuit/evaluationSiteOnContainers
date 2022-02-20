namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.HttpClients;

public class IdentityClientService
{
    private readonly HttpClient _client;
    private readonly ILogger<IdentityClientService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityClientService(HttpClient client, ILogger<IdentityClientService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<HttpResponseMessage> GetCommentsUserProfileAsync(List<string> userIds)
    {
        _logger.LogDebug("---- BackManage client call identity services: baseUrl:{url}", _client.BaseAddress);
        var callUrl = _client.BaseAddress + "api/v1/u/avatar/batch";

        try
        {
            var postBody = JsonContent.Create(userIds, MediaTypeHeaderValue.Parse("application/json"));

            var response = await _client.PostAsync(callUrl, postBody);

            if (response is null)
            {
                _logger.LogError("http response return null when callTo:{callUrl}, bodyInfo:{ids}", callUrl, userIds);
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

    public async Task<HttpResponseMessage> ApproveUserToEvaluatorAsync(string userId)
    {
        _logger.LogDebug("---- BackManage client call identity services: baseUrl:{url}", _client.BaseAddress);
        var callUrl = _client.BaseAddress + $"api/v1/u/approve/{userId}";

        try
        {
            var header = _httpContextAccessor.HttpContext.Request.Headers;
            _client.DefaultRequestHeaders.Add("Authorization", header["Authorization"].ToString());

            var response = await _client.PostAsync(callUrl, null);

            if (response is null)
            {
                _logger.LogError("http response return null when callTo:{callUrl}, userId:{id}", callUrl, userId);
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

    public async Task<HttpResponseMessage> RedrawUserToNormalUserAsync(string userId)
    {
        _logger.LogDebug("---- BackManage client call identity services: baseUrl:{url}", _client.BaseAddress);
        var callUrl = _client.BaseAddress + $"api/v1/u/redraw/{userId}";

        try
        {
            var header = _httpContextAccessor.HttpContext.Request.Headers;
            _client.DefaultRequestHeaders.Add("Authorization", header["Authorization"].ToString());

            var response = await _client.PostAsync(callUrl, null);

            if (response is null)
            {
                _logger.LogError("http response return null when callTo:{callUrl}, userId:{id}", callUrl, userId);
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

    public async Task<HttpResponseMessage> BannedUserAsync(string userId)
    {
        _logger.LogDebug("---- BackManage client call identity services: baseUrl:{url}", _client.BaseAddress);
        var callUrl = _client.BaseAddress + $"api/v1/u/ban/{userId}";

        try
        {
            var header = _httpContextAccessor.HttpContext.Request.Headers;
            _client.DefaultRequestHeaders.Add("Authorization", header["Authorization"].ToString());

            var response = await _client.PostAsync(callUrl, null);

            if (response is null)
            {
                _logger.LogError("http response return null when callTo:{callUrl}, userId:{id}", callUrl, userId);
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

    public async Task<HttpResponseMessage> RecoverUserToNormalUserAsync(string userId)
    {
        _logger.LogDebug("---- BackManage client call identity services: baseUrl:{url}", _client.BaseAddress);
        var callUrl = _client.BaseAddress + $"api/v1/u/recover/{userId}";

        try
        {
            var header = _httpContextAccessor.HttpContext.Request.Headers;
            _client.DefaultRequestHeaders.Add("Authorization", header["Authorization"].ToString());

            var response = await _client.PostAsync(callUrl, null);

            if (response is null)
            {
                _logger.LogError("http response return null when callTo:{callUrl}, userId:{id}", callUrl, userId);
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