namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models;

[Table("gamesdk_player")]
[Comment("保存用户购买的SDK信息")]
public class GameSDKForPlayer
{
    [Key]
    [Column("sdk_player_id")]
    [Comment("主键")]
    public int Id { get; set; }

    [Required]
    [Column("user_id")]
    [Comment("购买用户的Id")]
    public string UserId { get; set; }

    [Column("has_checked")]
    [Comment("是否已经被校验")]
    public bool? HasChecked { get; set; }

    [Column("check_time")]
    [Comment("校验的时间")]
    public DateTime? CheckTime { get; set; }

    [Required]
    [Column("sdk_id")]
    [Comment("游戏发放的sdk外键")]
    public long SDKItemId { get; set; }

    [NotMapped] 
    public GameItemSDK GameItemSDK { get; set; }
}