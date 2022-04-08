namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Controllers;

/// <summary>
/// 用户封禁信息接口
/// </summary>
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

    /// <summary>
    /// (管理员)获取被举报用户列表
    /// </summary>
    /// <param name="pageIndex">分页大小为10</param>
    /// <param name="status">checking-未处理 banned-已经封禁</param>
    /// <returns></returns>
    [HttpGet("list")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<BannedRecordDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetBannedUserListAsync([FromQuery] int pageIndex = 1, [FromQuery] BannedStatus status = BannedStatus.Checking)
    {
        var bannedCount = await _bannedService.CountBannedRecordAsync(status);
        if (bannedCount == 0) return NotFound();

        if (ParameterValidateHelper.IsInvalidPageIndex(bannedCount, _pageSize, pageIndex)) pageIndex = 1;

        var bannedRecords = await _bannedService.GetBannedRecordsAsync(pageIndex, _pageSize, status);

        // 调用Identity服务获取被封禁用户额外信息
        var userIds = bannedRecords.Select(x => x.UserId).ToList();
        using var response = await _identityHttpClient.GetUserProfileAsync(userIds);
        var userInfoDto = new List<UserAvatarDto>();
        if (response.IsSuccessStatusCode) userInfoDto = await response.Content.ReadFromJsonAsync<List<UserAvatarDto>>();

        var bannedToReturn = _mapper.Map<List<BannedRecordDto>>(bannedRecords);
        var model = new PaginatedItemsDtoModel<BannedRecordDto>(pageIndex, _pageSize, bannedCount, bannedToReturn, userInfoDto);
        return Ok(model);
    }

    /// <summary>
    /// (管理员)获取特定用户封禁信息
    /// </summary>
    /// <param name="id">举报记录Id</param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType(typeof(BannedRecordDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetBannedInfoAsync([FromRoute] int id)
    {
        var bannedInfo = await _bannedService.GetBannedRecordByIdAsync(id);
        if (bannedInfo == null) return NotFound();

        var infoToReturn = _mapper.Map<BannedRecordDto>(bannedInfo);
        return Ok(infoToReturn);
    }

    /// <summary>
    /// (用户)举报某用户
    /// </summary>
    /// <param name="addDto"></param>
    /// <returns></returns>
    /// <exception cref="BackManageDomainException"></exception>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostBannedInfoAsync([FromBody] BannedRecordAddDto addDto)
    {
        if (addDto == null) return BadRequest();
        var checkUserId = User.FindFirstValue("sub");

        // 每个用户允许举报另用户且一次
        var checkUserLink = await _bannedService.CheckUserLinkExistAsync(addDto.UserId, checkUserId);
        if (checkUserLink == true) return BadRequest("你已经举报过该用户啦 ^ ^");

        // 获取举报记录 若举报记录为空则新增
        var bannedRecord = await _bannedService.GetBannedRecordByUserIdAsync(addDto.UserId);
        if (bannedRecord == null)
        {
            var entityToAdd = new BannedRecord { UserId = addDto.UserId };
            var addResponse = await _bannedService.AddBannedRecordAsync(entityToAdd, checkUserId);
            return addResponse == true
                ? StatusCode(StatusCodes.Status201Created)
                : throw new BackManageDomainException($"user:{checkUserId} wanna ban {addDto.UserId} but add record fail");
        }
        // 举报记录不为空 则修改举报次数
        var updateResponse = await _bannedService.UpdateBannedRecordAsync(bannedRecord.Id, checkUserId);
        return updateResponse == true
            ? NoContent()
            : throw new BackManageDomainException($"user:{checkUserId} wanna ban {addDto.UserId} but update record fail");
    }

    /// <summary>
    /// (管理员)为用户解除封禁状态
    /// </summary>
    /// <param name="userId">举报记录Id</param>
    /// <returns></returns>
    /// <exception cref="BackManageDomainException"></exception>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> DeleteBannedInfoAsync([FromRoute] string userId)
    {
        var bannedRecord = await _bannedService.GetBannedRecordByUserIdAsync(userId);
        if (bannedRecord == null) return BadRequest();

        // 调用Identity服务 接触封禁用户
        using var response = await _identityHttpClient.RecoverUserToNormalUserAsync(userId);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("----- backmanage call identity successfully, cancel band user:{id} -----", userId);
            _logger.LogInformation("----- now change status in banned tables -----");

            var deleteResult = await _bannedService.DeleteBannedRecordAsync(bannedRecord);
            if (deleteResult == true)
            {
                _logger.LogInformation("----- user:{id} banned has been cancel now -----", userId);
                return NoContent();
            }
            else
            {
                _logger.LogInformation("----- progress error, now start to banned user:{id} -----", userId);
                var bandResult = await _identityHttpClient.BannedUserAsync(userId);
                if (!bandResult.IsSuccessStatusCode)
                    throw new BackManageDomainException("recover user occurred error, please check logging and fix it");
            }
        }

        throw new BackManageDomainException("recover user occurred error, because can't call identity microservice, pls retry");
    }

    /// <summary>
    /// (管理员)封禁用户
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <returns></returns>
    /// <exception cref="BackManageDomainException"></exception>
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

            var statusUpdateResult = await _bannedService.UpdateBannedStatusRecordAsync(bannedRecord.Id, User.FindFirstValue("nickname"));
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