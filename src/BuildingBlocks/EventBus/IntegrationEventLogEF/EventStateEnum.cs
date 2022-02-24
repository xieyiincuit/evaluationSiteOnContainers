namespace Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.IntegrationEventLogEF;

/// <summary>
///     事件状态
/// </summary>
public enum EventStateEnum
{
    NotPublished = 0,
    InProgress = 1,
    Published = 2,
    PublishedFailed = 3
}