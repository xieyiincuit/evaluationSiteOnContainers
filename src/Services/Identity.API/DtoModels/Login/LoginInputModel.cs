namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.Login;
public class LoginInputModel
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    public bool RememberLogin { get; set; }
    public string ReturnUrl { get; set; }
}
