namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.DtoModels;

public class ApproveRecordAddDto
{
    [Display(Name = "申请补充内容")]
    [Required(ErrorMessage = "required | 请填写{0}")]
    public string Body { get; set; }
}