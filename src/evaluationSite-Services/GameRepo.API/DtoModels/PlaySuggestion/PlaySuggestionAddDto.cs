namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class PlaySuggestionAddDto
{
    [Required(ErrorMessage = "required | 请填写建议的操作系统")]
    [MaxLength(50, ErrorMessage = "length | 不能超过50个字符")]
    public string OperationSystem { get; set; }

    [Required(ErrorMessage = "required | 请填写建议的CPU型号")]
    [MaxLength(50, ErrorMessage = "length | 不能超过50个字符")]
    [JsonPropertyName("cupName")]
    public string CPUName { get; set; }

    [Required(ErrorMessage = "required | 请填写建议的内存大小")]
    [Range(0, 64, ErrorMessage = "invalid | 非法参数: memorySize")]
    public double MemorySize { get; set; }

    [Required(ErrorMessage = "required | 请填写建议的磁盘大小")]
    [Range(0, 1000, ErrorMessage = "invalid | 非法参数: diskSize")]
    public double DiskSize { get; set; }

    [Required(ErrorMessage = "required | 请填写建议的显卡型号")]
    [MaxLength(50, ErrorMessage = "length | 不能超过50个字符")]
    public string GraphicsCard { get; set; }

    [Required(ErrorMessage = "required | gameId loss")]
    [Range(1, int.MaxValue, ErrorMessage = "invalid | 非法参数: gameId")]
    public int GameId { get; set; }
}
