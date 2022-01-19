namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.DtoModels;

public class GameCategoryAddDto
{
    [Required(ErrorMessage = "required | 请填写游戏类型名")]
    [MaxLength(50, ErrorMessage = "不能超过50个字符")]
    public string CategoryName  { get; set; }
}
