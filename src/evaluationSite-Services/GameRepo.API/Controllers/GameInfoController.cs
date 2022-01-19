namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/g")]
public class GameInfoController : ControllerBase
{
    private readonly IGameInfoService _gameInfoService;

    public GameInfoController(IGameInfoService gameInfoService)
    {
        _gameInfoService = gameInfoService;
    }
}
