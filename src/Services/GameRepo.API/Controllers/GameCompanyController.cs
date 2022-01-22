namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/g")]
public class GameCompanyController : ControllerBase
{
    private readonly IGameCompanyService _companyService;
    private readonly IMapper _mapper;
    private const int _pageSize = 10;

    public GameCompanyController(IGameCompanyService companyService, IMapper mapper)
    {
        _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateCompanyAsync([FromBody] GameCompanyAddDto companyAddDto)
    {
        if (companyAddDto == null) return BadRequest();

        var entityToAdd = _mapper.Map<GameCompany>(companyAddDto);

        await _companyService.AddGameCompanyAsync(entityToAdd);
        return CreatedAtRoute(nameof(GetCompanyByIdAsync), new { companyId = entityToAdd.Id }, null);
    }

    [HttpDelete]
    [Route("company/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
    {
        if (id <= 0 || id >= int.MaxValue) return BadRequest();

        var response = await _companyService.DeleteGameCompanyAsync(id);
        return response == true ? NoContent() : NotFound();
    }

    [HttpPut]
    [Route("company")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateCategoryAsync([FromBody] GameCompanyUpdateDto companyUpdateDto)
    {
        if (companyUpdateDto == null) return BadRequest();

        var entityToUpdate = _mapper.Map<GameCompany>(companyUpdateDto);
        await _companyService.UpdateGameCompanyAsync(entityToUpdate);
        return NoContent();
    }
}
