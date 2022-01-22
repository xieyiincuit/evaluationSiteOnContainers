namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.IntegrationEvents.Events;

//当游戏名称被更改时，此服务应发送集成事件通知测评服务，让测评服务将游戏名设为一致
public record GameNameChangedIntegrationEvent : IntegrationEvent
{
    public GameNameChangedIntegrationEvent(int gameId, string oldName, string newName)
    {
        GameId = gameId;
        OldName = oldName;
        NewName = newName;
    }

    public int GameId { get; set; }
    public string NewName { get; set; }
    public string OldName { get; set; }
}