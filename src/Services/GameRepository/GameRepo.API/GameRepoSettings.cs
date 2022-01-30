namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API;

public class GameRepoSettings
{
    public string DetailPics { get; set; }
    public string RoughPics { get; set; }
}

public class EventBusSettings
{
    public string Connection { get; set; }
    public string Port { get; set; }
    public string UserName { get; set; }
    public string PassWord { get; set; }
    public string RetryCount { get; set; }
}