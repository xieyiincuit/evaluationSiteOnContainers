namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Models;

[Table("gameinfo_tag")]
[Comment("游戏与标签的多对多链接表")]
public class GameInfoTag
{
    public int GameId { get; set; }
    public GameInfo GameInfo { get; set; }

    public int TagId { get; set; }
    public GameTag GameTag { get; set; }
}
