namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class ShopItemDtoToAdmin
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public float Discount { get; set; }
    public int AvailableStock { get; set; }
    public bool? TemporaryStopSell { get; set; }
    public int GameId { get; set; }
    public string GameName { get; set; }
    public string Picture { get; set; }
}