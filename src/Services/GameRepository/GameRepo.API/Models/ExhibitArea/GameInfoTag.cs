namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models;

[Table("gameinfo_tag")]
[Comment("游戏与标签的多对多链接表")]
public class GameInfoTag
{
    [Column("game_id")] [Comment("游戏id")] public int GameId { get; set; }

    public GameInfo GameInfo { get; set; }

    [Column("tag_id")] [Comment("标签id")] public int TagId { get; set; }

    public GameTag GameTag { get; set; }
}