namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.IntegrationEvents;

public class OrderIntegrationEventService : IOrderIntegrationEventService
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderIntegrationEventService> _logger;
    private volatile bool _disposedValue;

    public OrderIntegrationEventService(
        ILogger<OrderIntegrationEventService> logger,
        IEventBus eventBus)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task PublishThroughEventBusAsync(IntegrationEvent @event)
    {
        try
        {
            _logger.LogInformation(
                "----- Publishing integration event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})",
                @event.Id, Program.AppName, @event);
            _eventBus.Publish(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "ERROR Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})",
                @event.Id, Program.AppName, @event);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}