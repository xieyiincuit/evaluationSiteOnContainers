namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.Login;
public class LoginInputModel
{
    [Display(Name = "用户名")]
    [Required(ErrorMessage = "请输入用户名")]
    public string Username { get; set; }

    [Display(Name = "密码")]
    [Required(ErrorMessage = "请输入密码")]
    public string Password { get; set; }
    public bool RememberLogin { get; set; }
    public string ReturnUrl { get; set; }
}
