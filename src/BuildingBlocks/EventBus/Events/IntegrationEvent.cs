namespace Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBus.Events;

/// <summary>
/// 事件源基类
/// </summary>
public record IntegrationEvent
{
    // Integration Event（集成事件)                                                                  
    // 因为在微服务中事件的消费不再局限于当前领域内,而是多个微服务可能共享同一个事件,所以这里要和DDD中的领域事件区分开来.
    // 集成事件可用于跨多个微服务或外部系统同步领域状态,这是通过在微服务之外发布集成事件来实现的.                  

    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime createDate)
    {
        Id = id;
        CreationDate = createDate;
    }

    [JsonInclude]
    public Guid Id { get; private init; }

    [JsonInclude]
    public DateTime CreationDate { get; private init; }
}
