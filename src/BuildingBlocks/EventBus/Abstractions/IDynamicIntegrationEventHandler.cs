namespace Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBus.Abstractions;

/// <summary>
/// 动态类型的事件处理接口
/// 更具有灵活性 可以用于发布简单的事件源
/// </summary>
public interface IDynamicIntegrationEventHandler
{
    Task Handle(dynamic eventData);
}
