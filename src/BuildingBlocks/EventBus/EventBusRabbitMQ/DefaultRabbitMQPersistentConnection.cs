namespace Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusRabbitMQ;

public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
{
    //RabbitMQ连接实例
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;

    //重连次数
    private readonly int _retryCount;

    //同步锁目的是为了服务启动时只有一个服务启动连接程序
    private readonly object _syncLock = new();

    private IConnection _connection;
    private bool _disposed;

    public DefaultRabbitMQPersistentConnection(
        IConnectionFactory connectionFactory, ILogger<DefaultRabbitMQPersistentConnection> logger, int retryCount = 5)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _retryCount = retryCount;
    }

    public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

    //在连接成功后用于创建Channel
    public IModel CreateModel()
    {
        //若连接未成功，抛出异常
        if (!IsConnected)
            throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

        //返回Channel
        return _connection.CreateModel();
    }

    //关闭连接时Dispose
    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        try
        {
            _connection.ConnectionShutdown -= OnConnectionShutdown; //取消连接关闭事件
            _connection.CallbackException -= OnCallbackException; //取消回调异常事件
            _connection.ConnectionBlocked -= OnConnectionBlocked; //取消连接阻塞事件
            _connection.Dispose(); //释放连接
        }
        catch (IOException ex) //I/O释放失败
        {
            _logger.LogCritical(ex.ToString());
        }
    }

    //进行连接逻辑
    public bool TryConnect()
    {
        _logger.LogInformation("RabbitMQ Client is trying to connect");

        //同步锁，保持只有一个实例进行RabbitMQ连接逻辑
        lock (_syncLock)
        {
            //指数重试策略，使得连接服务可以从错误中恢复
            var policy = Policy.Handle<SocketException>() //连接出现SocketException异常
                .Or<BrokerUnreachableException>() //连接出现BrokerUnreachableException异常
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) =>
                    {
                        _logger.LogWarning(ex,
                            "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})",
                            $"{time.TotalSeconds:n1}", ex.Message);
                    } //等待并重试连接
                );

            //执行连接策略
            policy.Execute(() => { _connection = _connectionFactory.CreateConnection(); });

            //连接成功
            if (IsConnected)
            {
                _connection.ConnectionShutdown += OnConnectionShutdown; //绑定连接关闭事件
                _connection.CallbackException += OnCallbackException; //绑定回调异常事件
                _connection.ConnectionBlocked += OnConnectionBlocked; //绑定连接阻塞异常

                _logger.LogInformation(
                    "RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events",
                    _connection.Endpoint.HostName);

                return true;
            }

            _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

            return false;
        }
    }

    private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed) return;

        _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

        TryConnect();
    }

    private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
    {
        if (_disposed) return;

        _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

        TryConnect();
    }

    private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
    {
        if (_disposed) return;

        _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

        TryConnect();
    }
}