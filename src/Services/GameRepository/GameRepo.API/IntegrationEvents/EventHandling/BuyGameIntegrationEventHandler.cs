namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.IntegrationEvents.EventHandling;

public class BuyGameIntegrationEventHandler : 
    IIntegrationEventHandler<BuyGameIntegrationEvent>
{
    private readonly ILogger<BuyGameIntegrationEventHandler> _logger;
    private readonly IGameShopItemService _shopItemService;
    private readonly IGameInfoService _gameInfoService;
    private readonly IUnitOfWorkService _unitOfWorkService;

    public BuyGameIntegrationEventHandler(
        ILogger<BuyGameIntegrationEventHandler> logger,
        IGameShopItemService shopItemService,
        IGameInfoService gameInfoService,
        IUnitOfWorkService unitOfWorkService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _shopItemService = shopItemService ?? throw new ArgumentNullException(nameof(shopItemService));
        _gameInfoService = gameInfoService ?? throw new ArgumentNullException(nameof(gameInfoService));
        _unitOfWorkService = unitOfWorkService ?? throw new ArgumentNullException(nameof(unitOfWorkService));
    }

    public async Task Handle(BuyGameIntegrationEvent @event)
    {
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
        {
            _logger.LogInformation(
                "----- Handling integration event Begin: {IntegrationEventId} at {AppName} - {@IntegrationEvent}",
                @event.Id, Program.AppName, @event);

            var gameId = await _shopItemService.UpdateShopItemInfoWhenUserBuyAsync(@event.ShopItemId);
            await _gameInfoService.UpdateGameInfoWhenUserBuyAsync(gameId);
            await _unitOfWorkService.SaveEntitiesAsync();

            _logger.LogInformation(
                "----- Handling integration event End: {IntegrationEventId} at {AppName} - {@IntegrationEvent}",
                @event.Id, Program.AppName, @event);
        }
    }
}