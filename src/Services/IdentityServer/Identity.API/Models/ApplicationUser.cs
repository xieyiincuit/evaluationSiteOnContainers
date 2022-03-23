namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Models;

public class ApplicationUser : IdentityUser
{
    [Required(ErrorMessage = "请填写你的昵称")]
    [MaxLength(50, ErrorMessage = "不要超过五十个字符")]
    public string NickName { get; set; }

    [MaxLength(100)] public string Avatar { get; set; }

    public Gender? Sex { get; set; }

    [Required(ErrorMessage = "自定义你的密保问题用来找回你的密码")]
    [MaxLength(100, ErrorMessage = "长度不能超过一百个字符")]
    public string SecurityQuestion { get; set; }

    [Required(ErrorMessage = "请填写回答并牢记")]
    [MaxLength(100, ErrorMessage = "长度不能超过一百个字符")]
    public string SecurityAnswer { get; set; }

    [Required]
    public DateTime RegistrationDate { get; set; }

    public DateTime? LastChangeNameTime { get; set; }

    public DateTime? BirthDate { get; set; }

    [MaxLength(50, ErrorMessage = "不超过50字符")]
    public string Introduction { get; set; }
}

public enum Gender
{
    Female = 0,
    Male = 1
}