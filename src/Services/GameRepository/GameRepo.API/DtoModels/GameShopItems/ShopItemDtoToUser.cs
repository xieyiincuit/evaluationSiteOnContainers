namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class ShopItemDtoToUser
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public float Discount { get; set; }
    public decimal FinalPrice { get; set; }
    public string Picture { get; set; }


    public int GameId { get; set; }
    public string GameName { get; set; }
    public double GameScore { get; set; }
    public string GamePicture { get; set; }
    public DateTime SellTime { get; set; }
    public string GameCategory { get; set; }
}