namespace Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusRabbitMQ;

/// <summary>
/// Broke Connecter
/// </summary>
public interface IRabbitMQPersistentConnection : IDisposable
{
    bool IsConnected { get; }

    bool TryConnect();

    IModel CreateModel();
}