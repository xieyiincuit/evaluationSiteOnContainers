namespace Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase;

/// <summary>
///     使用内存进行存储事件源和事件处理的映射字典
/// </summary>
public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
{
    // 保存所有的事件处理类型
    private readonly List<Type> _eventTypes;

    // 定义的事件名称和事件订阅的字典映射（1:N）
    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;

    public InMemoryEventBusSubscriptionsManager()
    {
        _handlers = new Dictionary<string, List<SubscriptionInfo>>();
        _eventTypes = new List<Type>();
    }

    //定义事件移除后的事件处理
    public event EventHandler<string> OnEventRemoved;

    public bool IsEmpty => _handlers is {Count: 0};

    public void Clear()
    {
        _handlers.Clear();
    }


    // 添加动态类型事件订阅 (需要手动指定事件名称)
    public void AddDynamicSubscription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        DoAddSubscription(typeof(TH), eventName, true);
    }


    //添加强类型事件订阅（事件名称为事件源类型）
    public void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = GetEventKey<T>(); //Get event name

        DoAddSubscription(typeof(TH), eventName, false);

        // 新增事件订阅类型
        if (!_eventTypes.Contains(typeof(T))) _eventTypes.Add(typeof(T));
    }


    // 移除动态类型事件订阅
    public void RemoveDynamicSubscription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        var handlerToRemove = FindDynamicSubscriptionToRemove<TH>(eventName);
        DoRemoveHandler(eventName, handlerToRemove);
    }


    // 移除强类型事件订阅
    public void RemoveSubscription<T, TH>()
        where TH : IIntegrationEventHandler<T>
        where T : IntegrationEvent
    {
        var handlerToRemove = FindSubscriptionToRemove<T, TH>();
        var eventName = GetEventKey<T>();
        DoRemoveHandler(eventName, handlerToRemove);
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
    {
        var key = GetEventKey<T>();
        return GetHandlersForEvent(key);
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
    {
        return _handlers[eventName];
    }

    public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
    {
        var key = GetEventKey<T>();
        return HasSubscriptionsForEvent(key);
    }

    public bool HasSubscriptionsForEvent(string eventName)
    {
        return _handlers.ContainsKey(eventName);
    }

    public Type GetEventTypeByName(string eventName)
    {
        return _eventTypes.SingleOrDefault(t => t.Name == eventName);
    }

    public string GetEventKey<T>()
    {
        return typeof(T).Name;
    }


    // 给事件订阅表中新增类型为handlerType的事件
    private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic)
    {
        // 初始化映射表, 防止第一次添加事件时NullReference
        if (!HasSubscriptionsForEvent(eventName)) _handlers.Add(eventName, new List<SubscriptionInfo>());

        if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'",
                nameof(handlerType));

        if (isDynamic)
            _handlers[eventName].Add(SubscriptionInfo.Dynamic(handlerType));
        else
            _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
    }


    private void DoRemoveHandler(string eventName, SubscriptionInfo subsToRemove)
    {
        if (subsToRemove != null)
        {
            //移除映射表中的特定事件类型中的某个事件
            _handlers[eventName].Remove(subsToRemove);

            //如果该事件类型没有订阅的事件了, 则将该事件类型一并移除
            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);
                var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                if (eventType != null) _eventTypes.Remove(eventType);

                //当一个事件类型订阅的事件全部被移除后, 触发事件移除事件。
                RaiseOnEventRemoved(eventName);
            }
        }
    }

    private void RaiseOnEventRemoved(string eventName)
    {
        var handler = OnEventRemoved;
        handler?.Invoke(this, eventName);
    }

    private SubscriptionInfo FindDynamicSubscriptionToRemove<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        return DoFindSubscriptionToRemove(eventName, typeof(TH));
    }

    private SubscriptionInfo FindSubscriptionToRemove<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = GetEventKey<T>();
        return DoFindSubscriptionToRemove(eventName, typeof(TH));
    }

    private SubscriptionInfo DoFindSubscriptionToRemove(string eventName, Type handlerType)
    {
        if (!HasSubscriptionsForEvent(eventName))
            return null;

        return _handlers[eventName].SingleOrDefault(subscript => subscript.HandlerType == handlerType);
    }
}