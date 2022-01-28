namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Controllers;

[ApiController]
[Route("api/v1/u/")]
public class UserManageController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserManageController> _logger;
    private readonly IIdentityIntegrationEventService _identityIntegrationService;
    private readonly ApplicationDbContext _applicationDbContext;

    public UserManageController(
        UserManager<ApplicationUser> userManager,
        ILogger<UserManageController> logger,
        IIdentityIntegrationEventService identityIntegrationService,
        ApplicationDbContext applicationDbContext)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _logger = logger;
        _identityIntegrationService = identityIntegrationService ?? throw new ArgumentNullException(nameof(identityIntegrationService));
        _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
    }

    [HttpGet("info")]
    [Authorize(Roles = "normaluser, administrator, evaluator")]
    [ProducesResponseType(typeof(UserInfoDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetUserInfoAsync()
    {
        var currentUserId = User.FindFirstValue("sub");

        //非法访问他人Id或无效Id
        if (string.IsNullOrEmpty(currentUserId)) return BadRequest(
            new { Message = "can't find sub claim in current context" });

        var user = await _userManager.FindByIdAsync(currentUserId);
        if (user == null) return NotFound();

        var userDto = UserInfoMapping.MapToDtoModel(user);
        return Ok(userDto);
    }

    [HttpPut("info")]
    [Authorize(Roles = "normaluser, administrator, evaluator")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateUserInfoAsync([FromBody] UserInfoUpdateDto updateDto)
    {
        if (updateDto.UserId != User.FindFirstValue("sub")) return BadRequest();

        var userEntityForUpdate = await _userManager.FindByIdAsync(updateDto.UserId);

        //检查NickName是否更改, 若更改需要触发事件通知其他服务
        var oldNickName = userEntityForUpdate.NickName;
        var raiseNickNameChangedEvent = oldNickName != updateDto.NickName;

        //先判断该姓名是否被其他人员使用
        if (raiseNickNameChangedEvent)
        {
            var userForCheck = await _userManager.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.NickName == updateDto.NickName);

            //为true 说明该名字已被别人使用
            if (userForCheck != null && userForCheck.Id != userEntityForUpdate.Id)
                throw new IdentityDomainException("该昵称已被占用, 请尝试其他昵称");

            //更新用户信息
            userEntityForUpdate = UserInfoMapping.UpdateDtoMapToModel(updateDto, userEntityForUpdate);

            //发出集成事件
            _logger.LogInformation("----- User's NickNameChangedEvent Raised, Will Send a message to Event Bus");
            //1. 初始化集成事件，待事件总线发布。
            var nameChangedEvent = new NickNameChangedIntegrationEvent(
                userEntityForUpdate.Id, oldName: oldNickName, newName: updateDto.NickName);
            //2. 使用事务保证原子性的同时，在发布事件时同时记录事件日志。
            await _identityIntegrationService.SaveEventAndApplicationUserContextChangeAsync(nameChangedEvent);
            //3. 将该事件发布并修改该事件发布状态为已发布
            await _identityIntegrationService.PublishThroughEventBusAsync(nameChangedEvent);
        }
        else
        {
            //用户没有更新新的昵称，则只保存用户更新数据即可
            userEntityForUpdate = UserInfoMapping.UpdateDtoMapToModel(updateDto, userEntityForUpdate);
            _applicationDbContext.Users.Update(userEntityForUpdate);
            await _applicationDbContext.SaveChangesAsync();
        }

        return NoContent();
    }


}
