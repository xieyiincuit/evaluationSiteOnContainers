namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models;

[Table("game_tag")]
[Comment("游戏标签表")]
public class GameTag
{
    [Key]
    [Column("tag_id")]
    [Comment("主键")]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("tag_name")]
    [Comment("游戏标签名")]
    public string TagName { get; set; }

    [Column("is_deleted")]
    [Comment("逻辑删除")]
    [JsonIgnore]
    public bool? IsDeleted { get; set; }

    [JsonIgnore] public List<GameInfo> GameInfos { get; set; }

    [JsonIgnore] public List<GameInfoTag> GameInfoTags { get; set; }
}