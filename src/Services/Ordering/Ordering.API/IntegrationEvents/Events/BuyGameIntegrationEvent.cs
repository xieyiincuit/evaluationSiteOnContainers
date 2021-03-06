namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.IntegrationEvents.Events;

public record BuyGameIntegrationEvent : IntegrationEvent
{
    public BuyGameIntegrationEvent(int shopItemId)
    {
        ShopItemId = shopItemId;
    }
    public int ShopItemId { get; set; }
}