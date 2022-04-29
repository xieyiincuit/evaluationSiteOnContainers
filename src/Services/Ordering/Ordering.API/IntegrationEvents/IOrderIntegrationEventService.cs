namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.IntegrationEvents;

public interface IOrderIntegrationEventService
{
    /// <summary>
    ///     发布集成事件
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    Task PublishThroughEventBusAsync(IntegrationEvent @event);
}