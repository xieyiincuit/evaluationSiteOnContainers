namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.IntegrationEvents;

public interface IIdentityIntegrationEventService
{
    /// <summary>
    /// 将消息变更记录到事件日志
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    Task SaveEventAndApplicationUserContextChangeAsync(IntegrationEvent @event);

    /// <summary>
    /// 发布集成事件
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    Task PublishThroughEventBusAsync(IntegrationEvent @event);
}