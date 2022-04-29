namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.IntegrationEvents.Events;

public class BuyGameIntegrationEvent
{
    public BuyGameIntegrationEvent(int shopItemId, int gameInfoId)
    {
        this.ShopItemId = shopItemId;
        this.GameInfoId = gameInfoId;
    }

    public int ShopItemId { get; set; }
    public int GameInfoId { get; set; }
}