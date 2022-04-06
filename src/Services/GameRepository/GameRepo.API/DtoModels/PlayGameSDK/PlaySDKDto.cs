namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class PlaySDKDto
{
    public int Id { get; set; }
    public string SDKString { get; set; }
    public bool hasChecked { get; set; }

    public int GameId { get; set; }
    public string GameName { get; set; }
    public string GamePictrue { get; set; }

    public DateTime SendTime { get; set; }
}