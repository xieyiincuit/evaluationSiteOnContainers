namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.DtoModels;

public class BannedRecordAddDto
{
    [Display(Name = "举报用户Id")]
    [Required(ErrorMessage = "required | {0} loss")]
    [MaxLength(200, ErrorMessage = "length | {0}的长度不合法")]
    public string UserId { get; set; }
}