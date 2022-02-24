namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.IntegrationEvents;

public interface IGameRepoIntegrationEventService
{
    /// <summary>
    ///     将消息变更记录到事件日志
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    Task SaveEventAndGameRepoContextChangeAsync(IntegrationEvent @event);

    /// <summary>
    ///     发布集成事件
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    Task PublishThroughEventBusAsync(IntegrationEvent @event);
}