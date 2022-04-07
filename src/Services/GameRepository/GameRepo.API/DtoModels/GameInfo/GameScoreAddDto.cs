namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class GameScoreAddDto
{
    [Required(ErrorMessage = "required | gameId loss")]
    [Range(1, int.MaxValue, ErrorMessage = "invalid | 非法参数: id")]
    public int GameId { get; set; }

    [Display(Name = "游戏评分")]
    [Required(ErrorMessage = "required | 请选择{0}")]
    [Range(1, 10, ErrorMessage = "invalid | 非法参数: {0}过高或过低")]
    public double GameScore { get; set; }
}
