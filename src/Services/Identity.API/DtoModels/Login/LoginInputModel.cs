namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.Login;
public class LoginInputModel
{
    [Display(Name = "�û���")]
    [Required(ErrorMessage = "�������û���")]
    public string Username { get; set; }

    [Display(Name = "����")]
    [Required(ErrorMessage = "����������")]
    public string Password { get; set; }
    public bool RememberLogin { get; set; }
    public string ReturnUrl { get; set; }
}
