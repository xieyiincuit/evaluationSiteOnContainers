namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.DtoModels;

public class ApproveRecordDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime ApplyTime { get; set; }
    public ApproveStatus Status { get; set; }
}