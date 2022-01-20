namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Models;

[Table("game_type")]
[Comment("游戏类型分类表")]
public class GameCategory
{
    [Key]
    [Column("type_id"), Comment("主键")]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    [Column("category_name"), Comment("游戏类型名")]
    public string CategoryName { get; set; }

    [Column("is_deleted"), Comment("逻辑删除")]
    [JsonIgnore]
    public bool? IsDeleted { get; set; }
}
