namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/g")]
public class GameCategoryController : ControllerBase
{
    private readonly IGameCategory _categoryService;

    public GameCategoryController(IGameCategory categoryService)
    {
        _categoryService = categoryService;
    }
}
