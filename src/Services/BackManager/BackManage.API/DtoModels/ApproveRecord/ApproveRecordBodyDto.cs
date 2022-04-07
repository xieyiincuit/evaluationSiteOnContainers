namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.DtoModels;

public class ApproveRecordBodyDto
{
    public int Id { get; set; }
    public string Body { get; set; }
    public DateTime ApplyTime { get; set; }
    public ApproveStatus Status { get; set; }
}