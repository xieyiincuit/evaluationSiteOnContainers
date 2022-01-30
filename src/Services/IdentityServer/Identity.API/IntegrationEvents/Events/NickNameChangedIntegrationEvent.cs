namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.IntegrationEvents.Events;

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