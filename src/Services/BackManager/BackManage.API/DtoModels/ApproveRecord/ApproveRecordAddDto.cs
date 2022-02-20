namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.DtoModels;

public class ApproveRecordAddDto
{
    [Display(Name = "申请补充内容")]
    [Required(ErrorMessage = "required | 请填写{0}")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "length | {0}长度应在{2}至{1}之间")]
    public string Body { get; set; }
}