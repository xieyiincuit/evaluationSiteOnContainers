namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API;

public class OrderingSettings
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
