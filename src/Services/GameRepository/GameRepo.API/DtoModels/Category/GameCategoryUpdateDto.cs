namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class GameCategoryUpdateDto
{
    [Required(ErrorMessage = "required | id loss")]
    [Range(1, int.MaxValue, ErrorMessage = "invalid | 非法参数: id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "required | 请填写游戏类型名")]
    [MaxLength(50, ErrorMessage = "不能超过50个字符")]
    public string CategoryName { get; set; }
}