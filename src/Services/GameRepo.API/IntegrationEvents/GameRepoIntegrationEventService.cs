namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.IntegrationEvents;

public class GameRepoIntegrationEventService : IGameRepoIntegrationEventService
{
    private readonly ILogger<GameRepoIntegrationEventService> _logger;
    private readonly IEventBus _eventBus;
    private readonly GameRepoContext _repoContext;
    private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;

    private readonly IIntegrationEventLogService _eventLogService;
    private volatile bool _disposedValue;

    public GameRepoIntegrationEventService(
        ILogger<GameRepoIntegrationEventService> logger,
        IEventBus eventBus,
        GameRepoContext repoContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _repoContext = repoContext ?? throw new ArgumentNullException(nameof(repoContext));
        _integrationEventLogServiceFactory = integrationEventLogServiceFactory
                                             ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
        //事件日志持久化应该和事件的发起者保持一致
        _eventLogService = _integrationEventLogServiceFactory(_repoContext.Database.GetDbConnection());
    }

    public async Task SaveEventAndGameRepoContextChangeAsync(IntegrationEvent @event)
    {
        _logger.LogInformation("----- GameRepoIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}", @event.Id);

        //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
        await ResilientTransaction.New(_repoContext).ExecuteAsync(async () =>
        {
            // Achieving atomicity between original gamerepo database operation and the IntegrationEventLog use transaction
            await _repoContext.SaveChangesAsync();
            await _eventLogService.SaveEventAsync(@event, _repoContext.Database.CurrentTransaction);
        });
    }

    public async Task PublishThroughEventBusAsync(IntegrationEvent @event)
    {
        try
        {
            _logger.LogInformation("----- Publishing integration event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})", 
                @event.Id, Program.AppName, @event);

            await _eventLogService.MarkEventAsInProgressAsync(@event.Id);
            _eventBus.Publish(@event);
            await _eventLogService.MarkEventAsPublishedAsync(@event.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", 
                @event.Id, Program.AppName, @event);
            await _eventLogService.MarkEventAsFailedAsync(@event.Id);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        //此变量线程共享
        if (!_disposedValue)
        {
            if (disposing)
            {
                (_eventLogService as IDisposable)?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}