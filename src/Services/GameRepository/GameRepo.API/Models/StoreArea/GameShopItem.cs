namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models;

[Table("gameshop_items")]
[Comment("游戏售卖商品表")]
public class GameShopItem
{
    [Key]
    [Column("item_id")]
    [Comment("主键")]
    public int Id { get; set; }

    [Required]
    [Column("price")]
    [Comment("售价")]
    public decimal Price { get; set; }

    [Required]
    [Column("discount")]
    [Comment("折扣")]
    public float Discount { get; set; }

    [Required]
    [Range(1, 10000)]
    [Column("available_stock")]
    [Comment("库存量")]
    public int AvailableStock { get; set; }

    [Required]
    [Column("hotsell_point")]
    [Comment("销售热度")]
    public double HotSellPoint { get; set; }

    [Column("temporary_stopsell")]
    [Comment("暂停销售")]
    public bool? TemporaryStopSell { get; set; }

    [Column("sell_pictrue")]
    [Comment("商品图片")]
    public string? SellPictrue { get; set; }

    [Required]
    [Column("game_id")]
    [Comment("游戏信息外键")]
    public int GameInfoId { get; set; }

    [NotMapped] public GameInfo GameInfo { get; set; }

    [NotMapped] public List<GameItemSDK> GameSDKList { get; set; }
}