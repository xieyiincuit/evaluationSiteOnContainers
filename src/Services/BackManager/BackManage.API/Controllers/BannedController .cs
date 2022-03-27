namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Controllers;

[ApiController]
[Route("api/v1/back/banned")]
public class BannedController : ControllerBase
{
    private const int _pageSize = 10;
    private readonly IBannedService _bannedService;
    private readonly IdentityHttpClient _identityHttpClient;
    private readonly ILogger<BannedController> _logger;
    private readonly IMapper _mapper;

    public BannedController(
        ILogger<BannedController> logger, IBannedService bannedService,
        IdentityHttpClient identityHttpClient, IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _bannedService = bannedService ?? throw new ArgumentNullException(nameof(bannedService));
        _identityHttpClient = identityHttpClient ?? throw new ArgumentNullException(nameof(identityHttpClient));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet("list")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> GetBannedUserListAsync(
        [FromQuery] int pageIndex = 1, [FromQuery] BannedStatus status = BannedStatus.Checking)
    {
        var bannedCount = await _bannedService.CountBannedRecordAsync(status);
        if (bannedCount == 0) return NotFound();

        if (ParameterValidateHelper.IsInvalidPageIndex(bannedCount, _pageSize, pageIndex)) pageIndex = 1;

        var bannedRecords = await _bannedService.GetBannedRecordsAsync(pageIndex, _pageSize, status);

        var userIds = bannedRecords.Select(x => x.UserId).ToList();
        using var response = await _identityHttpClient.GetUserProfileAsync(userIds);
        var userInfoDto = new List<UserAvatarDto>();
        if (response.IsSuccessStatusCode) userInfoDto = await response.Content.ReadFromJsonAsync<List<UserAvatarDto>>();

        var bannedToReturn = _mapper.Map<List<BannedRecordDto>>(bannedRecords);
        var model = new PaginatedItemsDtoModel<BannedRecordDto>(pageIndex, _pageSize, bannedCount, bannedToReturn,
            userInfoDto);
        return Ok(model);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> GetBannedInfoAsync([FromRoute] int id)
    {
        var bannedInfo = await _bannedService.GetBannedRecordByIdAsync(id);
        if (bannedInfo == null) return NotFound();

        var infoToReturn = _mapper.Map<BannedRecordDto>(bannedInfo);
        return Ok(infoToReturn);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostBannedInfoAsync([FromBody] BannedRecordAddDto addDto)
    {
        if (addDto == null) return BadRequest();
        var checkUserId = User.FindFirstValue("sub");

        var checkUserLink = await _bannedService.CheckUserLinkExistAsync(addDto.UserId, checkUserId);
        if (checkUserLink == true) return BadRequest("你已经举报过该用户啦 ^ ^");

        var bannedRecord = await _bannedService.GetBannedRecordByUserIdAsync(addDto.UserId);
        if (bannedRecord == null)
        {
            var entityToAdd = new BannedRecord { UserId = addDto.UserId };
            var addResponse = await _bannedService.AddBannedRecordAsync(entityToAdd, checkUserId);
            return addResponse == true
                ? Ok()
                : throw new BackManageDomainException(
                    $"user:{checkUserId} wanna ban {addDto.UserId} but add record fail");
        }

        var updateResponse = await _bannedService.UpdateBannedRecordAsync(bannedRecord.Id, checkUserId);
        return updateResponse == true
            ? NoContent()
            : throw new BackManageDomainException(
                $"user:{checkUserId} wanna ban {addDto.UserId} but update record fail");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> DeleteBannedInfoAsync([FromRoute] int id)
    {
        var bannedRecord = await _bannedService.GetBannedRecordByIdAsync(id);
        if (bannedRecord == null) return BadRequest();

        using var response = await _identityHttpClient.RecoverUserToNormalUserAsync(bannedRecord.UserId);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("----- backmanage call identity successfully, cancel band user:{id} -----",
                bannedRecord.UserId);
            _logger.LogInformation("----- now change status in banned tables -----");

            var deleteResult = await _bannedService.DeleteBannedRecordAsync(bannedRecord);
            if (deleteResult == true)
            {
                _logger.LogInformation("----- user:{id} banned has been cancel now -----", bannedRecord.UserId);
                return NoContent();
            }

            _logger.LogInformation("----- progress error, now start to banned user:{id} -----", bannedRecord.UserId);
            var bandResult = await _identityHttpClient.BannedUserAsync(bannedRecord.UserId);
            if (!bandResult.IsSuccessStatusCode)
                throw new BackManageDomainException("recover user occurred error, please check logging and fix it");
        }

        throw new BackManageDomainException("recover user occurred error, because can't call identity microservice");
    }

    [HttpPost("ban/{userId}")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> BandUserAsync([FromRoute] string userId)
    {
        var bannedRecord = await _bannedService.GetBannedRecordByUserIdAsync(userId);
        if (bannedRecord == null) return BadRequest();

        using var response = await _identityHttpClient.BannedUserAsync(userId);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("----- backmanage call identity successfully, band user:{id} -----", userId);
            _logger.LogInformation("----- now change status in banned tables -----");

            var statusUpdateResult =
                await _bannedService.UpdateBannedStatusRecordAsync(bannedRecord.Id, User.FindFirstValue("nickname"));
            if (statusUpdateResult == true)
            {
                _logger.LogInformation("----- user:{id} has been banned now -----", userId);
                return Ok();
            }

            _logger.LogInformation("----- progress error, now start to recover banned user:{id} -----", userId);
            var recoverResult = await _identityHttpClient.RecoverUserToNormalUserAsync(userId);
            if (!recoverResult.IsSuccessStatusCode)
                throw new BackManageDomainException("banned user occurred error, please check logging and fix it");
        }

        throw new BackManageDomainException("banned user occurred error, because can't call identity microservice");
    }
}