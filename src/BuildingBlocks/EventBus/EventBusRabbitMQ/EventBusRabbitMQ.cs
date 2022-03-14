namespace Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusRabbitMQ;

//事件总线RabbitMQ具体实现
public class EventBusRabbitMQ : IEventBus, IDisposable
{
    // 声明该总线的 Broker Name
    private const string BROKER_NAME = "evaluation_event_bus";

    // 依赖注入Scope，通过该Scope我们可以跨程序集注入服务
    private const string AUTOFAC_SCOPE_NAME = "evaluation_event_bus";
    private readonly ILifetimeScope _autofac;
    private readonly ILogger<EventBusRabbitMQ> _logger;

    // Broker Connector
    private readonly IRabbitMQPersistentConnection _persistentConnection;
    private readonly int _retryCount;

    // 用于维护事件的订阅和注销, 以及订阅信息的持久化
    private readonly IEventBusSubscriptionsManager _subsManager;

    private IModel _consumerChannel;
    private string _queueName;

    public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
        ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager, string queueName = null, int retryCount = 5)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subsManager = subsManager;
        _queueName = queueName;
        _consumerChannel = CreateConsumerChannel();
        _autofac = autofac;
        _retryCount = retryCount;
        _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
    }

    //断开RabbitMQ后，清空内存订阅管理中的所有注册的事件
    public void Dispose()
    {
        if (_consumerChannel != null) _consumerChannel.Dispose();

        _subsManager.Clear();
    }

    /// <summary>
    ///     事件发布
    /// </summary>
    /// <param name="event"></param>
    public void Publish(IntegrationEvent @event)
    {
        //检查连接是否正常 若未连接先重试连接
        if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();

        //创建指数级的重试策略，允许Socket连接失败的可能，使得服务可恢复。
        var policy = RetryPolicy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})",
                        @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

        //获得事件具体的类型
        var eventName = @event.GetType().Name;

        _logger.LogDebug("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

        //创建Channel
        using var channel = _persistentConnection.CreateModel();
        _logger.LogDebug("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);

        //声明Channel名以及分发方式
        channel.ExchangeDeclare(BROKER_NAME, "direct");

        //格式化数据为Json格式
        var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions
        {
            WriteIndented = true
        });

        //执行Publish
        policy.Execute(() =>
        {
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2; // 消息持久化

            _logger.LogInformation("Publishing event to RabbitMQ: {EventId}", @event.Id);

            //将事件发布到对应的routingKey里
            //mandatory:
            //When a published message cannot be routed to any queue, and the publisher set the mandatory message property to true,
            //the message will be returned to it. The publisher must have a returned message handler set up in order to handle the return 
            channel.BasicPublish(
                BROKER_NAME,
                eventName,
                true,
                properties,
                body);
        });
    }

    public void SubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}", eventName,
            typeof(TH).GetGenericTypeName());

        DoInternalSubscription(eventName);
        _subsManager.AddDynamicSubscription<TH>(eventName);
        StartBasicConsume();
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();
        DoInternalSubscription(eventName);

        _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}",
            eventName, typeof(TH).GetGenericTypeName());

        _subsManager.AddSubscription<T, TH>();
        StartBasicConsume();
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();

        _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

        _subsManager.RemoveSubscription<T, TH>();
    }

    public void UnsubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        _subsManager.RemoveDynamicSubscription<TH>(eventName);
    }

    /// <summary>
    ///     当集成事件都被取消时，同时关闭RabbitMQ链接
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventName">事件源类型</param>
    private void SubsManager_OnEventRemoved(object sender, string eventName)
    {
        if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();

        using var channel = _persistentConnection.CreateModel();
        //解绑对应的queue
        channel.QueueUnbind(
            _queueName,
            BROKER_NAME,
            eventName);

        if (_subsManager.IsEmpty)
        {
            _queueName = string.Empty;
            _consumerChannel.Close();
        }
    }

    /// <summary>
    ///     事件订阅
    /// </summary>
    /// <param name="eventName"></param>
    private void DoInternalSubscription(string eventName)
    {
        var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
        if (!containsKey)
        {
            if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();

            _consumerChannel.QueueBind(
                _queueName,
                BROKER_NAME,
                eventName);
        }
    }

    /// <summary>
    ///     开始事件消费监听
    /// </summary>
    private void StartBasicConsume()
    {
        _logger.LogDebug("Starting RabbitMQ basic consume");

        if (_consumerChannel != null)
        {
            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

            consumer.Received += Consumer_Received;

            _consumerChannel.BasicConsume(
                _queueName,
                false,
                consumer);
        }
        else
        {
            _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
        }
    }

    /// <summary>
    ///     消息接受事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        //根据路由Key获取该事件的类型名
        var eventName = eventArgs.RoutingKey;
        //从MessageBody中序列化消息
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {
            //若消息中被定义成自定义exception，我们抛出错误并可用自定义逻辑解决。
            if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
            //处理事件
            await ProcessEvent(eventName, message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
        }

        // 该消费出现异常也确认消费成功
        // 在某些重要业务中需要处理死信队列
        _consumerChannel.BasicAck(eventArgs.DeliveryTag, false);
    }

    /// <summary>
    ///     开启事件监听
    /// </summary>
    /// <returns></returns>
    private IModel CreateConsumerChannel()
    {
        if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();

        _logger.LogDebug("Creating RabbitMQ consumer channel");

        var channel = _persistentConnection.CreateModel();

        channel.ExchangeDeclare(BROKER_NAME,
            "direct");

        channel.QueueDeclare(
            _queueName,
            true,
            false,
            false,
            null);

        channel.CallbackException += (_, ea) =>
        {
            _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

            _consumerChannel.Dispose();
            _consumerChannel = CreateConsumerChannel();
            StartBasicConsume();
        };

        return channel;
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        _logger.LogDebug("Processing RabbitMQ event: {EventName}", eventName);

        //如果在内存中存在该事件的注册，则开始处理该事件
        if (_subsManager.HasSubscriptionsForEvent(eventName))
        {
            //获取事件总线服务容器，所有的事件处理都被注册在该Scope中
            await using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);
            //从订阅管理中获取该事件的处理对象
            var subscriptions = _subsManager.GetHandlersForEvent(eventName);
            
            //遍历事件处理Handler
            foreach (var subscription in subscriptions)
                if (subscription.IsDynamic) //若是动态处理事件
                {
                    // 将该Handler转换为动态事件处理接口
                    var handler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                    if (handler == null) continue; //转换失败，则说明该事件类型未注册。

                    //获取message数据
                    using dynamic eventData = JsonDocument.Parse(message);
                    
                    //以下两个await方法是在数据迭代中异步执行可回调的方法
                    await Task.Yield();
                    await handler.Handle(eventData);
                }
                else //是具体的订阅事件类型
                {
                    // 解析出订阅的事件类型Handler
                    var handler = scope.ResolveOptional(subscription.HandlerType);
                    if (handler == null) continue;
                    
                    //从订阅管理中获取事件类型
                    var eventType = _subsManager.GetEventTypeByName(eventName);
                    //将该事件解析对应eventType的integrationEvent对象
                    var integrationEvent = JsonSerializer.Deserialize(message, eventType,
                        new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
                    
                    //利用反射映射出具体的泛型处理接口
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    //在数据迭代中异步执行可回调的方法
                    await Task.Yield();
                    //反射调用具体integrationEvent类型的Handle方法
                    await (Task) concreteType.GetMethod("Handle").Invoke(handler, new[] {integrationEvent});
                }
        }
        else
        {
            _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
        }
    }
}