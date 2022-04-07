﻿namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Controllers;

/// <summary>
/// 用户测评资格审批
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
        ILogger<ApproveController> logger, IApprovalService approvalService,
        IdentityHttpClient identityHttpClient, IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _approvalService = approvalService ?? throw new ArgumentNullException(nameof(approvalService));
        _identityHttpClient = identityHttpClient ?? throw new ArgumentNullException(nameof(identityHttpClient));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// 获取审批列表 返回审批用户信息
    /// </summary>
    /// <param name="status">默认获取未进行操作的审批</param>
    /// <param name="pageIndex">分页大小为10</param>
    /// <returns></returns>
    [HttpGet("list")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType(typeof(PaginatedItemsDtoModel<ApproveRecordDto>), (int)HttpStatusCode.OK)]
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
    /// 获取用户审批内容
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType(typeof(ApproveRecordBodyDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetApproveAsync([FromRoute] int id)
    {
        var approve = await _approvalService.GetApproveRecordByIdAsync(id);
        if (approve == null) return NotFound();

        var approveToReturn = _mapper.Map<ApproveRecordBodyDto>(approve);
        return Ok(approveToReturn);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSelfApproveAsync()
    {
        var currentUser = User.FindFirstValue("sub");

        var approve = await _approvalService.GetApproveRecordByUserIdAsync(currentUser);
        if (approve == null) return NotFound();

        var approveToReturn = _mapper.Map<ApproveRecordBodyDto>(approve);
        return Ok(approveToReturn);
    }

    [HttpPost]
    [Authorize(Roles = "normaluser")]
    public async Task<IActionResult> AddApproveApplyAsync([FromBody] ApproveRecordAddDto addDto)
    {
        if (addDto == null) return BadRequest();
        var userId = User.FindFirstValue("sub");
        var entity = _mapper.Map<ApproveRecord>(addDto);
        entity.UserId = userId;

        //检查该用户是否已经存在审批，若存在且不为已拒绝，不允许多次申请
        var checkApprove = await _approvalService.GetApproveRecordByUserIdAsync(userId);
        if (checkApprove != null && checkApprove.Status != ApproveStatus.Rejected) return BadRequest();

        var response = await _approvalService.AddApproveRecordAsync(entity);
        if (response != true)
            throw new BackManageDomainException($"user {User.FindFirstValue("nickname")} add approve apply fail");

        _logger.LogInformation("user:{Name} add a approve:{Id} to be a evaluator", User.FindFirstValue("nickname"), entity.Id);
        return Ok();
    }

    [HttpPut]
    [Authorize(Roles = "normaluser")]
    public async Task<IActionResult> UpdateApproveBodyAsync([FromBody] ApproveRecordUpdateDto updateDto)
    {
        if (updateDto == null) return BadRequest();
        var currentUserId = User.FindFirstValue("sub");

        var approve = await _approvalService.GetApproveRecordByUserIdAsync(currentUserId);

        if (approve == null)
        {
            var entityToAdd = new ApproveRecord { UserId = currentUserId, Body = updateDto.Body };
            var addResponse = await _approvalService.AddApproveRecordAsync(entityToAdd);
            return addResponse == true
                ? (IActionResult)Ok()
                : throw new BackManageDomainException($"user {User.FindFirstValue("nickname")} add approve apply fail");
        }
        else
        {
            if (approve.UserId != User.FindFirstValue("sub")) return BadRequest();
            var updateResponse = await _approvalService.UpdateApproveInfoAsync(approve.Id, updateDto.Body);
            return updateResponse == true
                ? (IActionResult)NoContent()
                : throw new BackManageDomainException($"user {User.FindFirstValue("nickname")} edit approve body fail");
        }
    }

    [HttpDelete]
    [Authorize(Roles = "normaluser")]
    public async Task<IActionResult> DeleteApproveApplyAsync()
    {
        var userId = User.FindFirstValue("sub");
        var approve = await _approvalService.GetApproveRecordByUserIdAsync(userId);

        if (approve == null) return NotFound();
        if (approve.Status == ApproveStatus.Approved) return BadRequest("你的申请已被批准，不可撤销。");

        var response = await _approvalService.DeleteApproveRecordAsync(approve);
        if (response != true)
            throw new BackManageDomainException($"user {User.FindFirstValue("nickname")} delete approve fail");

        _logger.LogInformation("user:{Name} redraw a approve:{Id} to be a evaluator", User.FindFirstValue("nickname"), approve.Id);
        return NoContent();
    }

    /// <summary>
    /// 拒绝该用户的审批
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="BackManageDomainException"></exception>
    [HttpPut("reject/{userId}")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> RejectUserToEvaluatorAsync([FromRoute] string userId)
    {
        var approve = await _approvalService.GetApproveRecordByUserIdAsync(userId);
        if (approve == null) return BadRequest();

        var applyUser = User.FindFirstValue("nickname");
        var statusUpdateResult = await _approvalService.UpdateApproveStatusAsync(approve.Id, ApproveStatus.Rejected, applyUser);
        if (statusUpdateResult != true)
            throw new BackManageDomainException("reject user occurred error, please check logging and fix it");

        _logger.LogInformation("----- user:{Id} has been reject to a evaluator -----", userId);
        return NoContent();
    }

    /// <summary>
    /// 同意该用户的审批
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="BackManageDomainException"></exception>
    [HttpPut("check/{userId}")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> ApprovedUserToEvaluatorAsync([FromRoute] string userId)
    {
        var approve = await _approvalService.GetApproveRecordByUserIdAsync(userId);
        if (approve == null) return BadRequest();

        using var response = await _identityHttpClient.ApproveUserToEvaluatorAsync(userId);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("----- backmanage call identity successfully, make user:{Id} be a evaluator -----",
                userId);
            _logger.LogInformation("----- now change status in approve tables -----");

            var applyUser = User.FindFirstValue("nickname");
            var statusUpdateResult =
                await _approvalService.UpdateApproveStatusAsync(approve.Id, ApproveStatus.Approved, applyUser);
            if (statusUpdateResult == true)
            {
                _logger.LogInformation("----- user:{Id} has been a evaluator now -----", userId);
                return Ok();
            }

            _logger.LogInformation("----- progress error, now start to redraw approve user:{Id} -----", userId);
            var redrawResult = await _identityHttpClient.RedrawUserToNormalUserAsync(userId);
            if (!redrawResult.IsSuccessStatusCode)
                throw new BackManageDomainException("approve user occurred error, please check logging and fix it");
        }

        throw new BackManageDomainException("approve user occurred error, because can't call identity microservice");
    }

    /// <summary>
    /// 撤回该用户的审批
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

        using var response = await _identityHttpClient.RedrawUserToNormalUserAsync(userId);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(
                "----- backmanage call identity successfully, redraw user:{Id} to a normaluser -----", userId);
            _logger.LogInformation("----- now change status in approve tables -----");

            var applyUser = User.FindFirstValue("nickname");
            var statusUpdateResult =
                await _approvalService.UpdateApproveStatusAsync(approve.Id, ApproveStatus.Rejected, applyUser);
            if (statusUpdateResult == true)
            {
                _logger.LogInformation("----- user:{Id} has been a normaluser now -----", userId);
                return Ok();
            }

            _logger.LogInformation("----- progress error, now start to rollback approve user:{Id} -----", userId);
            var approveResult = await _identityHttpClient.ApproveUserToEvaluatorAsync(userId);
            if (!approveResult.IsSuccessStatusCode)
                throw new BackManageDomainException("approve user occurred error, please check logging and fix it");
        }

        throw new BackManageDomainException("approve user occurred error, because can't call identity microservice");
    }
}