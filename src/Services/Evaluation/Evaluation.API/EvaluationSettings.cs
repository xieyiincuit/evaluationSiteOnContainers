namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API;

public class EvaluationSettings
{
    public string ArticlePicBaseUrl { get; set; }
    public string DescriptionPicBaseUrl { get; set; }
    public bool UseCustomizationData { get; set; }
    public bool IsMeshClient { get; set; }
}

public class EventBusSettings
{
    public string Connection { get; set; }
    public string Port { get; set; }
    public string UserName { get; set; }
    public string PassWord { get; set; }
    public string RetryCount { get; set; }
}