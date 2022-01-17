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

    [MaxLength(1000)]
    [Column("description"), Comment("游戏描述")]
    public string? Description { get; set; }

    [Column("details_picture"), Comment("游戏展示图大图")]
    public string? DetailsPicture { get; set; }

    [Column("routh_picture"), Comment("游戏展示图小图")]
    public string? RoughPicture { get; set; }

    [Column("average_score"), Comment("游戏评分")]
    public double? AverageScore { get; set; }

    [Column("sell_time"), Comment("上市时间")]
    public DateTime? SellTime { get; set; }

    [Column("dev_company"), Comment("开发公司")]
    public string DevCompany { get; set; }


    [Column("type_id"), Comment("类型外键")]
    public int TypeId { get; set; }
    public GameCategory GameCategory { get; set; }

    public List<GameTag> GameTags { get; set; }
}
