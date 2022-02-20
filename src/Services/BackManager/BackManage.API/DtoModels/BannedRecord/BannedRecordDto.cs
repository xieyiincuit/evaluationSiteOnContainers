namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.DtoModels;

public class BannedRecordDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int ReportCount { get; set; }
    public DateTime? BannedTime { get; set; }
    public BannedStatus Status { get; set; }
}