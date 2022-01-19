namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/g")]
public class GameCompanyController : ControllerBase
{
    private readonly IGameCompany _companyService;

    public GameCompanyController(IGameCompany companyService)
    {
        _companyService = companyService;
    }
}
