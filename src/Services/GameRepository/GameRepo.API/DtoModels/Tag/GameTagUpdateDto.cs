namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class GameTagUpdateDto
{
    [Required(ErrorMessage = "required | tag id")]
    [Range(1, int.MaxValue, ErrorMessage = "invalid | 非法的id参数")]
    public int Id { get; set; }

    [Required(ErrorMessage = "required | 请填写游戏标签名")]
    [MaxLength(20, ErrorMessage = "不能超过20个字符")]
    public string TagName { get; set; }
}