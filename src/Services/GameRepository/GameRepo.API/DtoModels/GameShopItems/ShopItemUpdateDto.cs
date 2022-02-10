namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class ShopItemUpdateDto
{
    [Required(ErrorMessage = "required | itemId loss")]
    [Range(1, int.MaxValue, ErrorMessage = "invalid | 非法参数: id")]
    public int Id { get; set; }

    [Display(Name = "售价")]
    [Required(ErrorMessage = "required | 请填写商品{0}")]
    [Range(1, 10000, ErrorMessage = "invalid | 非法{0}")]
    public decimal Price { get; set; }

    [Display(Name = "折扣")]
    [Required(ErrorMessage = "required | 请填写商品{0}")]
    [Range(1, 100, ErrorMessage = "invalid | 非法{0}")]
    public float Discount { get; set; }
}
