﻿namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.DtoModels;

public class PlaySuggestionUpdateDto
{
    [Required(ErrorMessage = "required | id loss")]
    [Range(1, int.MaxValue, ErrorMessage = "invalid | 非法参数: id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "required | 请填写建议的操作系统")]
    [MaxLength(50, ErrorMessage = "length | 不能超过50个字符")]
    public string OperationSystem { get; set; }

    [Required(ErrorMessage = "required | 请填写建议的CPU型号")]
    [MaxLength(50, ErrorMessage = "length | 不能超过50个字符")]
    [JsonPropertyName("cupName")]
    public string CPUName { get; set; }

    [Required(ErrorMessage = "required | 请填写建议的内存大小")]
    [Range(0, 64, ErrorMessage = "invaild | 非法参数: memorySize")]
    public double MemorySize { get; set; }

    [Required(ErrorMessage = "required | 请填写建议的磁盘大小")]
    [Range(0, 1000, ErrorMessage = "invaild | 非法参数: diskSize")]
    public double DiskSize { get; set; }

    [Required(ErrorMessage = "required | 请填写建议的显卡型号")]
    [MaxLength(50, ErrorMessage = "length | 不能超过50个字符")]
    public string GraphicsCard { get; set; }
}
