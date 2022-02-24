namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class SDKPlayerAddDto
{
    [Required(ErrorMessage = "required | shopItemId loss")]
    [Range(1, int.MaxValue, ErrorMessage = "invalid | 非法参数: shopItemId")]
    public int ShopItemId { get; set; }

    [Required(ErrorMessage = "required | userId loss")]
    public string UserId { get; set; }
}