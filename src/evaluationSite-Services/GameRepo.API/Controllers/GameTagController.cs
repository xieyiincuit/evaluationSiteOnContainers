namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/g")]
public class GameTagController : ControllerBase
{
    private readonly IGameTag _tagService;

    public GameTagController(IGameTag tagService)
    {
        _tagService = tagService;
    }
}
