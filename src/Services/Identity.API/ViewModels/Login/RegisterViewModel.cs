namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.ViewModels.Login;

public class RegisterViewModel
{
    [Required(ErrorMessage = "请填写邮箱")]
    [EmailAddress]
    [Display(Name = "邮箱")]
    public string Email { get; init; }

    [Required(ErrorMessage = "请填写密码")]
    [StringLength(50, ErrorMessage = " {0}的长度在{2}到{1}之间.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "密码")]
    public string Password { get; init; }

    [DataType(DataType.Password)]
    [Display(Name = "重复密码")]
    [Compare("Password", ErrorMessage = "两次输入密码不同")]
    public string ConfirmPassword { get; init; }

    public ApplicationUser User { get; init; }
}