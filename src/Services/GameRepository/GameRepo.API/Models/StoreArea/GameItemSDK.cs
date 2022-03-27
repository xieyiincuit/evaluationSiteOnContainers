namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models;

[Table("gameitem_sdk")]
[Comment("sdk存放表, 负责给玩家发送未发送的sdk")]
public class GameItemSDK
{
    [Key]
    [Column("sdk_id")]
    [Comment("SDK主键")]
    public long Id { get; set; }

    [Required]
    [MaxLength(450)]
    [Column("sdk_string")]
    [Comment("游戏SDK码")]
    public string SDKString { get; set; }

    [Column("has_send")]
    [Comment("是否已卖出")]
    public bool? HasSend { get; set; }

    [Column("sent_time")]
    [Comment("发送时间")]
    public DateTime? SendTime { get; set; }

    [Required]
    [Column("item_id")]
    [Comment("游戏商品外键")]
    public int GameItemId { get; set; }

    [Column("row_version")]
    [Comment("行版本")]
    public DateTime RowVersion { get; set; }

    [NotMapped][JsonIgnore] public GameShopItem GameShopItem { get; set; }

    [NotMapped][JsonIgnore] public GameSDKForPlayer GameSDKForPlayer { get; set; }
}