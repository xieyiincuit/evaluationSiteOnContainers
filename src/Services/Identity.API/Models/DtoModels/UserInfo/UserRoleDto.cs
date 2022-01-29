namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.UserInfo;

public class UserRoleDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string NickName { get; set; }
    public string Avatar { get; set; }
    public DateTime RegisterTime { get; set; }
    public string Role { get; set; }
}