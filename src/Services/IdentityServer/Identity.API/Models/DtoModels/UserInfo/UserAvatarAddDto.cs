namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.UserInfo;

public class UserAvatarAddDto
{
    [Required(ErrorMessage = "required | 非法参数: userId")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "required | 请上传你的头像")]
    public IFormFile File { get; set; }
}