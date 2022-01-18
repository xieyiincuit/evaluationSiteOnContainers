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

    [Required, MaxLength(1000)]
    [Column("description"), Comment("游戏描述")]
    public string Description { get; set; }

    [Column("details_picture"), Comment("游戏展示图大图")]
    public string? DetailsPicture { get; set; }

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

    public GameCompany GameCompany { get; set; }

    public GameCategory GameCategory { get; set; }

    //一对一
    public PlaySuggestion PlaySuggestion { get; set; }

    public List<GameTag> GameTags { get; set; }
    public List<GameInfoTag> GameInfoTags { get; set; }
}
