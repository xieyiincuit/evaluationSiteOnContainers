namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Controllers;

/// <summary>
/// 用户测评资格审批接口
/// </summary>
[ApiController]
[Route("api/v1/back/approve")]
public class ApproveController : ControllerBase
{
    private const int _pageSize = 10;
    private readonly IApprovalService _approvalService;
    private readonly IdentityHttpClient _identityHttpClient;
    private readonly ILogger<ApproveController> _logger;
    private readonly IMapper _mapper;

    public ApproveController(
        ILogger<ApproveController> logger,
        IApprovalService approvalService,
        IdentityHttpClient identityHttpClient,
        IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _approvalService = approvalService ?? throw new ArgumentNullException(nameof(approvalService));
        _identityHttpClient = identityHttpClient ?? throw new ArgumentNullException(nameof(identityHttpClient));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// 管理员——获取审批列表
    /// </summary>
    /// <param name="status">默认获取未进行操作的审批</param>
    /// <param name="pageIndex">分页大小为10</param>
    /// <returns></returns>
    [HttpGet("list")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<ApproveRecordDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetApproveUserListAsync(
        [FromQuery] ApproveStatus status = ApproveStatus.Progressing,
        [FromQuery] int pageIndex = 1)
    {
        var approveCount = await _approvalService.CountApproveRecordByTypeAsync(status);
        if (approveCount == 0) return NotFound();

        if (ParameterValidateHelper.IsInvalidPageIndex(approveCount, _pageSize, pageIndex)) pageIndex = 1;

        var approves = await _approvalService.GetApproveRecordsAsync(pageIndex, _pageSize, status);

        var userIds = approves.Select(x => x.UserId).ToList();
        using var response = await _identityHttpClient.GetUserProfileAsync(userIds);
        var userInfoDto = new List<UserAvatarDto>();
        if (response.IsSuccessStatusCode) userInfoDto = await response.Content.ReadFromJsonAsync<List<UserAvatarDto>>();

        var approveToReturn = _mapper.Map<List<ApproveRecordDto>>(approves);
        var model = new PaginatedItemsDtoModel<ApproveRecordDto>(pageIndex, _pageSize, approveCount, approveToReturn, userInfoDto);
        return Ok(model);
    }

    /// <summary>
    /// 管理员——获取用户的审批内容
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}", Name = nameof(GetApproveAsync))]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType(typeof(ApproveRecordBodyDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetApproveAsync([FromRoute] int id)
    {
        var approve = await _approvalService.GetApproveRecordByIdAsync(id);
        if (approve == null) return NotFound();

        var approveToReturn = _mapper.Map<ApproveRecordBodyDto>(approve);
        return Ok(approveToReturn);
    }

    /// <summary>
    /// 用户——获取自己的测评资格申请内容
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApproveRecordBodyDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetSelfApproveAsync()
    {
        var currentUser = User.FindFirstValue("sub");

        var approve = await _approvalService.GetApproveRecordByUserIdAsync(currentUser);
        if (approve == null) return NotFound();

        var approveToReturn = _mapper.Map<ApproveRecordBodyDto>(approve);
        return Ok(approveToReturn);
    }

    /// <summary>
    /// 用户——提交自己的测评资格申请
    /// </summary>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    /// <exception cref="BackManageDomainException"></exception>
    [HttpPut]
    [Authorize(Roles = "normaluser")]
    public async Task<IActionResult> UpdateApproveBodyAsync([FromBody] ApproveRecordAddOrUpdateDto updateDto)
    {
        if (updateDto == null) return BadRequest();
        var currentUserId = User.FindFirstValue("sub");

        // 获取当前用户的申请信息
        var approve = await _approvalService.GetApproveRecordByUserIdAsync(currentUserId);

        // 若未申请则新增记录
        if (approve == null)
        {
            var entityToAdd = new ApproveRecord { UserId = currentUserId, Body = updateDto.Body };
            var addResponse = await _approvalService.AddApproveRecordAsync(entityToAdd);
            return addResponse == true
                ? (IActionResult)Created(nameof(GetApproveAsync), new { id = entityToAdd.Id })
                : throw new BackManageDomainException($"user {User.FindFirstValue("nickname")} add approve apply fail");
        }
        // 若已有申请记录则修改记录
        else
        {
            if (approve.UserId != User.FindFirstValue("sub")) return BadRequest();
            var updateResponse = await _approvalService.UpdateApproveInfoAsync(approve.Id, updateDto.Body);
            return updateResponse == true
                ? (IActionResult)NoContent()
                : throw new BackManageDomainException($"user {User.FindFirstValue("nickname")} update approve body fail");
        }
    }

    /// <summary>
    /// 用户——撤销自己的测评资格申请
    /// </summary>
    /// <returns></returns>
    /// <exception cref="BackManageDomainException"></exception>
    [HttpDelete]
    [Authorize(Roles = "normaluser, evaluator")]
    public async Task<IActionResult> DeleteApproveApplyAsync()
    {
        var userId = User.FindFirstValue("sub");
        var approve = await _approvalService.GetApproveRecordByUserIdAsync(userId);

        if (approve == null) return NotFound();
        // 申请已被批准则不允许删除申请记录
        if (approve.Status == ApproveStatus.Approved) return BadRequest();

        var response = await _approvalService.DeleteApproveRecordAsync(approve);
        if (response != true)
            throw new BackManageDomainException($"user {User.FindFirstValue("nickname")} delete approve fail");

        _logger.LogInformation("user:{Name} redraw a approve:{Id} to be a evaluator", User.FindFirstValue("nickname"), approve.Id);
        return NoContent();
    }

    /// <summary>
    /// 管理员——拒绝用户的测评资格申请
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <returns></returns>
    /// <exception cref="BackManageDomainException"></exception>
    [HttpPut("reject/{userId}")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> RejectUserToEvaluatorAsync([FromRoute] string userId)
    {
        // 获取该用户的申请
        var approve = await _approvalService.GetApproveRecordByUserIdAsync(userId);
        if (approve == null) return BadRequest();

        // 获取当前管理员信息
        var applyUser = User.FindFirstValue("nickname");

        // 更新用户的申请状态为拒绝
        var statusUpdateResult = await _approvalService.UpdateApproveStatusAsync(approve.Id, ApproveStatus.Rejected, applyUser);
        if (statusUpdateResult != true)
            throw new BackManageDomainException("reject user occurred error, please check logging and fix it");

        _logger.LogInformation("----- user:{Id} has been reject to a evaluator -----", userId);
        return NoContent();
    }

    /// <summary>
    /// 管理员——同意用户的测评资格审批
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <returns></returns>
    /// <exception cref="BackManageDomainException"></exception>
    [HttpPut("check/{userId}")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> ApprovedUserToEvaluatorAsync([FromRoute] string userId)
    {
        var approve = await _approvalService.GetApproveRecordByUserIdAsync(userId);
        if (approve == null) return BadRequest();

        // 调用Identity服务 提升该用户的身份
        using var response = await _identityHttpClient.ApproveUserToEvaluatorAsync(userId);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("----- backmanage call identity successfully, make user:{Id} be a evaluator -----", userId);
            _logger.LogInformation("----- now change status in approve tables -----");

            var applyUser = User.FindFirstValue("nickname");
            var statusUpdateResult = await _approvalService.UpdateApproveStatusAsync(approve.Id, ApproveStatus.Approved, applyUser);
            // 保持业务数据原子性 若成功则返回
            if (statusUpdateResult == true)
            {
                _logger.LogInformation("----- user:{Id} has been a evaluator now -----", userId);
                return NoContent();
            }
            else
            {
                // 若业务失败 则调用Identity服务回退该用户身份
                _logger.LogInformation("----- progress error, now start to redraw approve user:{Id} -----", userId);
                var redrawResult = await _identityHttpClient.RedrawUserToNormalUserAsync(userId);
                if (!redrawResult.IsSuccessStatusCode)
                    throw new BackManageDomainException("approve user occurred error, please check logging and fix it");
            }
        }
        throw new BackManageDomainException("approve user occurred error, because can't call identity microservice, pls retry");
    }

    /// <summary>
    /// 管理员——撤回用户的测评资格
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="BackManageDomainException"></exception>
    [HttpPut("redraw/{userId}")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> RedrawUserToNormalUserAsync([FromRoute] string userId)
    {
        var approve = await _approvalService.GetApproveRecordByUserIdAsync(userId);
        if (approve == null) return BadRequest();

        // 调用Identity服务 撤回该用户的身份
        using var response = await _identityHttpClient.RedrawUserToNormalUserAsync(userId);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("----- backmanage call identity successfully, redraw user:{Id} to a normaluser -----", userId);
            _logger.LogInformation("----- now change status in approve tables -----");

            var applyUser = User.FindFirstValue("nickname");
            var statusUpdateResult = await _approvalService.UpdateApproveStatusAsync(approve.Id, ApproveStatus.Rejected, applyUser);

            // 保持业务数据原子性 若成功则返回
            if (statusUpdateResult == true)
            {
                _logger.LogInformation("----- user:{Id} has been a normaluser now -----", userId);
                return NoContent();
            }
            else
            {
                // 若业务失败 则调用Identity服务回退该用户身份
                _logger.LogInformation("----- progress error, now start to rollback approve user:{Id} -----", userId);
                var approveResult = await _identityHttpClient.ApproveUserToEvaluatorAsync(userId);
                if (!approveResult.IsSuccessStatusCode)
                    throw new BackManageDomainException("approve user occurred error, please check logging and fix it");
            }
        }

        throw new BackManageDomainException("approve user occurred error, because can't call identity microservice, pls retry");
    }
}