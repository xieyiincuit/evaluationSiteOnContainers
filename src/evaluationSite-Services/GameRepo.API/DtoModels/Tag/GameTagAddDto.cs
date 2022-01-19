namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.DtoModels;

public class GameTagAddDto
{
    [Required(ErrorMessage = "required | 请填写游戏标签名")]
    [MaxLength(20, ErrorMessage = "不能超过20个字符")]
    public string TagName { get; set; }
}
