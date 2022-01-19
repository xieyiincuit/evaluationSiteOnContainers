namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/g")]
public class GameInfoController : ControllerBase
{
    private readonly IGameInfo _gameInfoService;

    public GameInfoController(IGameInfo gameInfoService)
    {
        _gameInfoService = gameInfoService;
    }
}
