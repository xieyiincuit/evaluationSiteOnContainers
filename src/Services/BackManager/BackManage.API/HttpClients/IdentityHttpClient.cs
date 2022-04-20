namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.HttpClients;

public class IdentityHttpClient
{
    private readonly HttpClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<IdentityHttpClient> _logger;

    public IdentityHttpClient(HttpClient client, ILogger<IdentityHttpClient> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<HttpResponseMessage> GetUserProfileAsync(List<string> userIds)
    {
        _logger.LogDebug("---- BackManage client call identity services: baseUrl:{url}", _client.BaseAddress);
        var callUrl = _client.BaseAddress + "api/v1/user/avatar/batch";

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
        var callUrl = _client.BaseAddress + $"api/v1/user/approve/{userId}";

        try
        {
            // 该操作需要进行身份权限认证
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
        var callUrl = _client.BaseAddress + $"api/v1/user/redraw/{userId}";

        try
        {
            // 该操作需要进行身份权限认证
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
        var callUrl = _client.BaseAddress + $"api/v1/user/ban/{userId}";

        try
        {
            // 该操作需要进行身份权限认证
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
        var callUrl = _client.BaseAddress + $"api/v1/user/recover/{userId}";

        try
        {
            // 该操作需要进行身份权限认证
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


    public async Task<HttpResponseMessage> CountUserAsync()
    {
        _logger.LogDebug("---- BackManage client call identity services: baseUrl:{url}", _client.BaseAddress);
        var callUrl = _client.BaseAddress + "api/v1/user/count";

        try
        {
            var header = _httpContextAccessor.HttpContext.Request.Headers;
            _client.DefaultRequestHeaders.Add("Authorization", header["Authorization"].ToString());

            var response = await _client.GetAsync(callUrl);

            if (response is null)
            {
                _logger.LogError("http response return null when callTo:{callUrl}", callUrl);
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