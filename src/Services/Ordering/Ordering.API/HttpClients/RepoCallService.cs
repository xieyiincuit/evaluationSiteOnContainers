namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.HttpClients;

public class RepoCallService
{
    private readonly HttpClient _httpClient;

    public RepoCallService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


}