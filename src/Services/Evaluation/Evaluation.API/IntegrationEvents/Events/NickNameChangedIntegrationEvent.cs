namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.IntegrationEvents.Events;

/// <summary>
///     用户名昵称改变
/// </summary>
public record NickNameChangedIntegrationEvent : IntegrationEvent
{
    public NickNameChangedIntegrationEvent(string userId, string oldName, string newName)
    {
        UserId = userId;
        OldName = oldName;
        NewName = newName;
    }

    public string UserId { get; set; }
    public string OldName { get; set; }
    public string NewName { get; set; }
}