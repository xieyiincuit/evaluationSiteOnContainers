namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.ViewModels.Login;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "请输入你的邮箱地址")]
    [EmailAddress(ErrorMessage = "邮箱格式错误")]
    [Display(Name = "邮箱地址")]
    public string Email { get; init; }

    [Required(ErrorMessage = "请输入绑定的手机号码")]
    [Phone(ErrorMessage = "手机号格式错误")]
    [StringLength(11, ErrorMessage = "手机格式错误", MinimumLength = 11)]
    [Display(Name = "该账号绑定手机")]
    public string PhoneNumber { get; set; }

    [Display(Name = "安全问题")] public string Question { get; set; }

    [Display(Name = "密保答案")] public string Answer { get; set; }
}