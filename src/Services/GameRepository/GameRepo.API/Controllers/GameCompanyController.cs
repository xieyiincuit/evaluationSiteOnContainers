namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/game")]
public class GameCompanyController : ControllerBase
{
    private const int _pageSize = 10;
    private readonly IGameCompanyService _companyService;
    private readonly ILogger<GameCompany> _logger;
    private readonly IMapper _mapper;

    public GameCompanyController(
        IGameCompanyService companyService,
        IMapper mapper,
        ILogger<GameCompany> logger)
    {
        _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Route("companies")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<GameCompany>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCompaniesAsync([FromQuery] int pageIndex = 1)
    {
        var totalCompanies = await _companyService.CountCompanyAsync();
        if (ParameterValidateHelper.IsInvalidPageIndex(totalCompanies, _pageSize, pageIndex)) pageIndex = 1;

        var companies = await _companyService.GetGameCompaniesAsync(pageIndex, _pageSize);
        if (!companies.Any()) return NotFound();

        var model = new PaginatedItemsDtoModel<GameCompany>(pageIndex, _pageSize, totalCompanies, companies);
        return Ok(model);
    }

    [HttpGet("company/{companyId:int}", Name = nameof(GetCompanyByIdAsync))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GameCompany), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCompanyByIdAsync([FromRoute] int companyId)
    {
        if (companyId <= 0 || companyId >= int.MaxValue) return BadRequest();

        var company = await _companyService.GetGameCompanyAsync(companyId);

        if (company == null) return NotFound();
        return Ok(company);
    }

    [HttpPost]
    [Route("company")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateCompanyAsync([FromBody] GameCompanyAddDto companyAddDto)
    {
        if (companyAddDto == null) return BadRequest();

        var entityToAdd = _mapper.Map<GameCompany>(companyAddDto);
        await _companyService.AddGameCompanyAsync(entityToAdd);

        _logger.LogInformation(
            $"administrator: id:{User.FindFirst("sub").Value}, name:{User.Identity.Name} add a company -> companyName:{companyAddDto.CompanyName}");

        return CreatedAtRoute(nameof(GetCompanyByIdAsync), new { companyId = entityToAdd.Id }, null);
    }

    [HttpDelete]
    [Route("company/{id:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();

        _logger.LogInformation(
            $"administrator: id:{User.FindFirst("sub").Value}, name:{User.Identity.Name} delete a company -> companyId:{id}");

        var response = await _companyService.DeleteGameCompanyAsync(id);
        return response == true ? NoContent() : NotFound();
    }

    [HttpPut]
    [Route("company")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateCategoryAsync([FromBody] GameCompanyUpdateDto companyUpdateDto)
    {
        if (companyUpdateDto == null) return BadRequest();

        var entityToUpdate = await _companyService.GetGameCompanyAsync(companyUpdateDto.Id);
        if (entityToUpdate == null) return NotFound();

        _logger.LogInformation(
            $"administrator: id:{User.FindFirst("sub").Value}, name:{User.Identity.Name} update a company -> old:{entityToUpdate.CompanyName}, new:{companyUpdateDto.CompanyName}");

        _mapper.Map(companyUpdateDto, entityToUpdate);
        await _companyService.UpdateGameCompanyAsync(entityToUpdate);
        return NoContent();
    }
}