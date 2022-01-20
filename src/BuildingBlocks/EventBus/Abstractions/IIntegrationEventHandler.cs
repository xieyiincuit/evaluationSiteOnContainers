namespace Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBus.Abstractions;

/// <summary>
/// 强类型事件处理接口
/// </summary>
/// <typeparam name="TIntegrationEvent"></typeparam>
public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}

public interface IIntegrationEventHandler
{
}
