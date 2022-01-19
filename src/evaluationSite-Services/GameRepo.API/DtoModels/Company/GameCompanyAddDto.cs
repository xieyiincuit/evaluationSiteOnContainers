namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.DtoModels;

public class GameCompanyAddDto
{
    [Required(ErrorMessage = "required | 请填写游戏公司名")]
    [MaxLength(50, ErrorMessage = "不能超过50个字符")]
    public string CompanyName { get; set; }
}
