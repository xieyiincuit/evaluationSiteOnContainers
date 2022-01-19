namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Models;

[Table("game_info")]
[Comment("游戏信息表")]
public class GameInfo
{
    [Key]
    [Column("game_id"), Comment("主键")]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    [Column("name"), Comment("游戏名")]
    public string Name { get; set; }

    [Required, MaxLength(500)]
    [Column("description"), Comment("游戏描述")]
    public string Description { get; set; }

    [MaxLength(200)]
    [Column("details_picture"), Comment("游戏展示图大图")]
    public string? DetailsPicture { get; set; }

    [MaxLength(200)]
    [Column("routh_picture"), Comment("游戏展示图小图")]
    public string? RoughPicture { get; set; }

    [Column("average_score"), Comment("游戏评分")]
    public double? AverageScore { get; set; }

    [Column("sell_time"), Comment("发售时间")]
    public DateTime? SellTime { get; set; }

    [Required, MaxLength(100)]
    [Column("dev_company"), Comment("开发公司")]
    public string DevCompany { get; set; }

    [Required, MaxLength(50)]
    [Column("support_platform"), Comment("游玩平台")]
    public string SupportPlatform { get; set; }

    [Column("hot_points"), Comment("游戏热度")]
    public long? HotPoints { get; set; }

    [ForeignKey("company_id"), Comment("发行公司外键")]
    public GameCompany GameCompany { get; set; }

    [ForeignKey("category_id"), Comment("游戏类型外键")]
    public GameCategory GameCategory { get; set; }

    //一对一
    public PlaySuggestion PlaySuggestion { get; set; }

    public List<GameTag> GameTags { get; set; }
    public List<GameInfoTag> GameInfoTags { get; set; }
}
