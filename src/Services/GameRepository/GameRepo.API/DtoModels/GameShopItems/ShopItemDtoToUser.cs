namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class ShopItemDtoToUser
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public float Discount { get; set; }
    public decimal FinalPrice { get; set; }
    public int GameId { get; set; }
    public string GameName { get; set; }
    public string Picture { get; set; }
}