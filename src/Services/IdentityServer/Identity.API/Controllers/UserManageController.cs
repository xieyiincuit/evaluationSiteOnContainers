﻿namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Controllers;

[ApiController]
[Route("api/v1/u/")]
public class UserManageController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserManageController> _logger;
    private readonly IIdentityIntegrationEventService _identityIntegrationService;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserManageController(
        UserManager<ApplicationUser> userManager,
        ILogger<UserManageController> logger,
        IIdentityIntegrationEventService identityIntegrationService,
        ApplicationDbContext applicationDbContext,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _logger = logger;
        _identityIntegrationService = identityIntegrationService ?? throw new ArgumentNullException(nameof(identityIntegrationService));
        _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    [HttpGet("info")]
    [Authorize(Roles = "normaluser, administrator, evaluator")]
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
                throw new IdentityDomainException("你心仪的昵称被其他玩家使用啦, 试试其他名字吧^ ^");

            //更新用户信息
            userEntityForUpdate = UserInfoMapping.UpdateDtoMapToModel(updateDto, userEntityForUpdate);
            userEntityForUpdate.LastChangeNameTime = DateTime.Now.ToLocalTime(); //TODO 可做时间范围内改名限制

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

    [HttpGet("list")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> GetUserInfoListAsync([FromQuery] int pageIndex = 1)
    {
        const int pageSize = 10;

        var userCount = await _userManager.Users.CountAsync();

        if (ParameterValidateHelper.IsInvalidPageIndex(userCount, pageSize, pageIndex)) pageIndex = 1;

        var userListDtos = await _userManager.Users
            .Select(x => new UserListDto
            {
                Id = x.Id,
                Avatar = x.Avatar,
                NickName = x.NickName,
                Email = x.Email,
                UserName = x.UserName,
                RegisterTime = x.RegistrationDate
            })
            .AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(x => x.RegisterTime)
            .ToListAsync();

        var model = new PaginatedItemsDtoModel<UserListDto>(pageIndex, pageSize, userCount, userListDtos);
        return Ok(model);
    }

    [HttpGet("role")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> GetNormalUserListAsync([FromQuery] int pageIndex = 1, [FromQuery] string roleSelect = "normaluser")
    {
        const int pageSize = 10;

        var roles = await _applicationDbContext.Roles
            .Where(x => x.NormalizedName != "administrator".ToUpper())
            .Select(x => x.Name)
            .AsNoTracking()
            .ToListAsync();
        if (!roles.Contains(roleSelect)) BadRequest();


        var role = await _applicationDbContext.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Name == roleSelect);
        var selectRolesCount = await _applicationDbContext.UserRoles.CountAsync(x => x.RoleId == role.Id);

        if (ParameterValidateHelper.IsInvalidPageIndex(selectRolesCount, pageSize, pageIndex)) pageIndex = 1;

        var userIds = await _applicationDbContext.UserRoles
            .Where(x => x.RoleId == role.Id)
            .Select(x => x.UserId)
            .AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var userList = new List<UserRoleDto>();
        foreach (var id in userIds)
        {
            var temp = await _applicationDbContext.Users
                .Where(x => x.Id == id)
                .Select(x => new UserRoleDto() { Id = x.Id, NickName = x.NickName, UserName = x.UserName, RegisterTime = x.RegistrationDate, Role = role.Name, Avatar = x.Avatar })
                .AsNoTracking()
                .OrderBy(x => x.RegisterTime)
                .FirstOrDefaultAsync();
            userList.Add(temp);
        }

        var model = new PaginatedItemsDtoModel<UserRoleDto>(pageIndex, pageSize, selectRolesCount, userList);
        return Ok(model);
    }

    [HttpPost("ban/{uid}")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> BanUserAsync([FromRoute] string uid)
    {
        var user = await _userManager.FindByIdAsync(uid);
        if (user == null) return BadRequest();

        var banRole = await _roleManager.FindByNameAsync("forbiddenuser");
        var normalRole = await _roleManager.FindByNameAsync("normaluser");
        var userRole = await _applicationDbContext.UserRoles.FindAsync(user.Id, normalRole.Id);

        //在RoleUser Table中 两个属性都是主键，所以需要先删除在重新建立联系
        if (userRole != null)
        {
            _applicationDbContext.Remove(userRole);
            await _applicationDbContext.SaveChangesAsync();
        }

        var newRoleLink = new IdentityUserRole<string> { RoleId = banRole.Id, UserId = user.Id };
        await _applicationDbContext.UserRoles.AddAsync(newRoleLink);

        var result = await _applicationDbContext.SaveChangesAsync();
        if (result <= 0) throw new IdentityDomainException($"ban user failed -> userName: {user.UserName}");
        return NoContent();
    }

    [HttpPost("approve/{uid}")]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> ApproveUserAsync([FromRoute] string uid)
    {
        var user = await _userManager.FindByIdAsync(uid);
        if (user == null) return BadRequest();

        var evaluatorRole = await _roleManager.FindByNameAsync("evaluator");
        var normalRole = await _roleManager.FindByNameAsync("normaluser");
        var userRole = await _applicationDbContext.UserRoles.FindAsync(user.Id, normalRole.Id);

        //在RoleUser Table中 两个属性都是主键，所以需要先删除在重新建立联系
        if (userRole != null)
        {
            _applicationDbContext.Remove(userRole);
            await _applicationDbContext.SaveChangesAsync();
        }

        var newRoleLink = new IdentityUserRole<string> { RoleId = evaluatorRole.Id, UserId = user.Id };
        await _applicationDbContext.UserRoles.AddAsync(newRoleLink);

        var result = await _applicationDbContext.SaveChangesAsync();
        if (result <= 0) throw new IdentityDomainException($"approve user failed -> userName: {user.UserName}");
        return NoContent();
    }
}