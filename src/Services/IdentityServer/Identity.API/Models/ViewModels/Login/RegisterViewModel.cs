namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.ViewModels.Login;

public class RegisterViewModel
{
    [Required(ErrorMessage = "请填写邮箱")]
    [EmailAddress]
    [Display(Name = "邮箱")]
    public string Email { get; init; }

    [Display(Name = "电话号码")]
    [Phone]
    [Required(ErrorMessage = "请填写你的电话号码")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "请填写密码")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,30}$"
        , ErrorMessage = "至少8个字符，至少1个字母，1个数字和1个特殊字符(@$!%*#?&)")]
    [StringLength(30, ErrorMessage = " {0}的长度在{2}到{1}之间.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "密码")]
    public string Password { get; init; }

    [DataType(DataType.Password)]
    [Display(Name = "确认密码")]
    [Compare("Password", ErrorMessage = "两次输入密码不同")]
    public string ConfirmPassword { get; init; }

    public ApplicationUser User { get; init; }
}