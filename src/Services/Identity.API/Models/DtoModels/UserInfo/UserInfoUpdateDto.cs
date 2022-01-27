namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.UserInfo;

public class UserInfoUpdateDto
{
    [Required(ErrorMessage = "invalid | 非法参数: userId")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "required | 请输入昵称")]
    [MaxLength(50, ErrorMessage = "length | 昵称不要超过50个字符")]
    public string NickName { get; set; }

    [Required(ErrorMessage = "required | 请选择性别")]
    public Gender? Sex { get; set; }

    public int? BirthOfYear { get; set; }
    public int? BirthOfMonth { get; set; }
    public int? BirthOfDay { get; set; }

    public string Introduction { get; set; }
}