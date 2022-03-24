namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.UserInfo;

public class UserPasswordDto
{
    [Required(ErrorMessage = "请填写旧密码")]
    [DataType(DataType.Password)]
    [Display(Name = "旧密码")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "请填写新密码")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[.@$!%*#?&])[A-Za-z\d$.@$!%*#?&]{8,30}$"
        , ErrorMessage = "至少8个字符，至少1个字母，1个数字和1个特殊字符(@$!%*#?&)")]
    [DataType(DataType.Password)]
    [Display(Name = "新密码")]
    public string NewPassword { get; init; }

    [DataType(DataType.Password)]
    [Display(Name = "确认密码")]
    [Compare("NewPassword", ErrorMessage = "两次输入密码不同")]
    public string CheckPassword { get; init; }
}