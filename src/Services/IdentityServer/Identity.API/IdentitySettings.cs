namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API;

public class IdentitySettings
{
}

public class EventBusSettings
{
    public string Connection { get; set; }
    public string Port { get; set; }
    public string UserName { get; set; }
    public string PassWord { get; set; }
    public string RetryCount { get; set; }
}