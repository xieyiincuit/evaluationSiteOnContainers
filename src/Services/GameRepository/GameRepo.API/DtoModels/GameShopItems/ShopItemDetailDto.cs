namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class ShopItemDetailDto
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public float Discount { get; set; }
    public decimal FinalPrice { get; set; }
    public string Picture { get; set; }


    public int GameId { get; set; }
    public string GameName { get; set; }
    public string GameDescription { get; set; }
    public string GamePicture { get; set; }
    public DateTime SellTime { get; set; }
    public string GameCategory { get; set; }
    public string GameIssue { get; set; }
}