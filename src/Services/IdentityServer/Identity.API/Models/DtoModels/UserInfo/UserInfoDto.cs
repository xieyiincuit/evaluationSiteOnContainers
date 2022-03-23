namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.UserInfo;

public class UserInfoDto
{
    public string UserId { get; set; }
    public string Avatar { get; set; }
    public string NickName { get; set; }
    public Gender? Sex { get; set; }

    public DateTime? BirthDate { get; set; }
    public string Introduction { get; set; }
}