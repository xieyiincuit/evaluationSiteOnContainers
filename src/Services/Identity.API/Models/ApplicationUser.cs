namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string NickName { get; set; }

    [Required]
    [MaxLength(100)]
    public string Avatar { get; set; }

    [Required]
    public Gender Sex { get; set; }

    [Required]
    [MaxLength(100)]
    public string SecurityQuestion { get; set; }

    [Required]
    [MaxLength(100)]
    public string SecurityAnswer { get; set; }

    [Required]
    public DateTime RegistrationDate { get; set; }

    public int? BirthOfYear { get; set; }

    public int? BirthOfMonth { get; set; }
    public int? BirthOfDay { get; set; }

    public string Introduction { get; set; }
}

public enum Gender
{
    Female = 0,
    Male = 1
}
