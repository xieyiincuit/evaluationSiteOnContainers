namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.IntegrationEvents.Events;

public record BuyGameIntegrationEvent : IntegrationEvent
{
    public BuyGameIntegrationEvent(int shopItemId)
    {
        this.ShopItemId = shopItemId;
    }
    public int ShopItemId { get; set; }
}