namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class GameCompanyUpdateDto
{
    [Required(ErrorMessage = "required | id loss")]
    [Range(1, int.MaxValue, ErrorMessage = "invalid | 非法参数: id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "required | 请填写游戏公司名")]
    [MaxLength(50, ErrorMessage = "length | 不能超过50个字符")]
    public string CompanyName { get; set; }
}