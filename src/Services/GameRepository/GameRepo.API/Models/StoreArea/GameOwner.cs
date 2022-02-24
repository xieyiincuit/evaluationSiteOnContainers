namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models;

[Table("game_owner")]
[Comment("玩家游戏拥有表")]
public class GameOwner
{
    [Column("user_id")] [Comment("用户id")] public string UserId { get; set; }

    [Column("game_id")]
    [Comment("游戏信息id")]
    public int GameId { get; set; }

    [NotMapped] public GameInfo GameInfo { get; set; }
}