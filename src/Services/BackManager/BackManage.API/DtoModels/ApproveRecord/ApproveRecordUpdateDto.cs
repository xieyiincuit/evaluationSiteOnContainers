namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.DtoModels;

public class ApproveRecordUpdateDto
{
    [Required(ErrorMessage = "required | id loss")]
    [Range(1, int.MaxValue, ErrorMessage = "invalid | 非法参数: id")]
    public int Id { get; set; }

    [Display(Name = "申请补充内容")]
    [Required(ErrorMessage = "required | 请填写{0}")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "length | {0}长度应在{2}至{1}之间")]
    public string Body { get; set; }
}