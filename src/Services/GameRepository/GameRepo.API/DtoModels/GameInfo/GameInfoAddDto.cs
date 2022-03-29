using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class GameInfoAddDto
{
    [Display(Name = "游戏名")]
    [Required(ErrorMessage = "required | 请填写{0}")]
    [MaxLength(50, ErrorMessage = "length | {0}不应超过50个字符")]
    public string Name { get; set; }

    [Display(Name = "游戏描述")]
    [Required(ErrorMessage = "required | 请填写{0}")]
    [MaxLength(1000, ErrorMessage = "length | {0}应在1000个字符内")]
    [MinLength(50, ErrorMessage = "length | {0}过短, 应超过50个字符")]
    public string Description { get; set; }

    [Display(Name = "展示大图")]
    [MaxLength(200, ErrorMessage = "length | {0}的uri过长, 控制在200字符以内")]
    public string? DetailsPicture { get; set; }

    [Display(Name = "略缩小图")]
    [MaxLength(200, ErrorMessage = "length | {0}的uri过长, 控制在200字符以内")]
    public string? RoughPicture { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "发售时间")]
    public DateTime? SellTime { get; set; }

    [Display(Name = "游玩平台")]
    [Required(ErrorMessage = "required | 请填写{0}")]
    [MaxLength(50, ErrorMessage = "length | {0}建议应在50字符以内")]
    public string SupportPlatform { get; set; }

    [Display(Name = "游戏类别")]
    [Required(ErrorMessage = "required | 请选择{0}")]
    [Range(0, int.MaxValue, ErrorMessage = "invalid | 非法参数: categoryId")]
    public int? CategoryId { get; set; }

    [Display(Name = "发行公司")]
    [Required(ErrorMessage = "required | 请选择{0}")]
    [Range(0, int.MaxValue, ErrorMessage = "invalid | 非法参数: companyId")]
    public int? CompanyId { get; set; }
}