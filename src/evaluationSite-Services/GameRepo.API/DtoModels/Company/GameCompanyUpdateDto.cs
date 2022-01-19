namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.DtoModels;

public class GameCompanyUpdateDto
{
    [Required(ErrorMessage = "required | 请填写company id")]
    [Range(1, int.MaxValue, ErrorMessage = "invalid | 非法的id参数")]
    public int Id { get; set; }

    [Required(ErrorMessage = "required | 请填写游戏公司名")]
    [MaxLength(50, ErrorMessage = "不能超过50个字符")]
    public string CompanyName { get; set; }
}
