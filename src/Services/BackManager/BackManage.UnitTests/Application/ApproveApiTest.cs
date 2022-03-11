using Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.DtoModels;
using Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Infrastructure;

namespace BackManage.UnitTests.Application;

public class ApproveApiTest
{
    private readonly Mock<IApprovalService> _approvalServiceMock;
    private readonly Mock<ILogger<ApproveController>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;

    //Mock IdentityClientService Parameters
    private readonly Mock<IdentityClientService> _identityClientMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<IdentityClientService>> _identityLoggerMock;
    private readonly Mock<IHttpContextAccessor> _contextAccessorMock;

    public ApproveApiTest()
    {
        _approvalServiceMock = new Mock<IApprovalService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<ApproveController>>();
        _httpClient = new HttpClient();
        _identityLoggerMock = new Mock<ILogger<IdentityClientService>>();
        _contextAccessorMock = new Mock<IHttpContextAccessor>();
        _identityClientMock = new Mock<IdentityClientService>(_httpClient, _identityLoggerMock.Object, _contextAccessorMock.Object);
    }

    [Fact]
    public async Task Get_user_approve_list_success()
    {
        //Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var status = ApproveStatus.Progressing;

        _approvalServiceMock
            .Setup(x => x.CountApproveRecordByTypeAsync(It.Is<ApproveStatus>(s => s == status)))
            .ReturnsAsync(10);

        _approvalServiceMock
            .Setup(x => x.GetApproveRecordsAsync(It.Is<int>(i => i == pageIndex), It.Is<int>(i => i == pageSize), It.Is<ApproveStatus>(s => s == status)))
            .ReturnsAsync(GetUserApproveFakeList());

        //Action
        var approveController = new ApproveController(
            _loggerMock.Object,
            _approvalServiceMock.Object,
            _identityClientMock.Object,
            _mapperMock.Object);

        var actionResult = await approveController.GetApproveUserListAsync();

        //Assert
        Assert.Equal(((OkObjectResult)actionResult).StatusCode, (int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_approve_item_success()
    {
        //Arrange
        var approveItemId = 1;

        _approvalServiceMock
            .Setup(x => x.GetApproveRecordByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(GetUserApproveFakeItem());

        //Action
        var approveController = new ApproveController(
            _loggerMock.Object,
            _approvalServiceMock.Object,
            _identityClientMock.Object,
            _mapperMock.Object);

        var actionResult = await approveController.GetApproveAsync(approveItemId);

        //Assert
        Assert.Equal(((OkObjectResult)actionResult).StatusCode, (int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_approve_item_notfound()
    {
        //Arrange
        var approveItemId = 1;

        _approvalServiceMock
            .Setup(x => x.GetApproveRecordByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(GetUserApproveNullItem());

        //Action
        var approveController = new ApproveController(
            _loggerMock.Object,
            _approvalServiceMock.Object,
            _identityClientMock.Object,
            _mapperMock.Object);

        var actionResult = await approveController.GetApproveAsync(approveItemId);

        //Assert
        Assert.Equal(((NotFoundResult)actionResult).StatusCode, (int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_user_approve_notfound()
    {
        //Arrange
        var fakeUserId = "fakeString";

        _approvalServiceMock
            .Setup(x => x.GetApproveRecordByUserIdAsync(It.Is<string>(id => id.Equals(fakeUserId))))
            .ReturnsAsync(GetUserApproveFakeItem());

        //Action
        var approveController = new ApproveController(
            _loggerMock.Object,
            _approvalServiceMock.Object,
            _identityClientMock.Object,
            _mapperMock.Object);

        approveController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() { User = GetFakeUserClaimsPrincipal() }
        };

        var actionResult = await approveController.GetSelfApproveAsync();

        //Assert
        Assert.Equal(((NotFoundResult)actionResult).StatusCode, (int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_user_approve_success()
    {
        //Arrange
        var correctSub = "correctSub";

        _approvalServiceMock
            .Setup(x => x.GetApproveRecordByUserIdAsync(It.Is<string>(id => id.Equals(correctSub))))
            .ReturnsAsync(GetUserApproveFakeItem());

        //Action
        var approveController = new ApproveController(
            _loggerMock.Object,
            _approvalServiceMock.Object,
            _identityClientMock.Object,
            _mapperMock.Object);

        approveController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() { User = GetExistUserClaimsPrincipal() }
        };

        var actionResult = await approveController.GetSelfApproveAsync();

        //Assert
        Assert.Equal(((OkObjectResult)actionResult).StatusCode, (int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task Add_user_approve_success()
    {
        //Arrange
        var correctSub = "correctSub";
        var addObject = new ApproveRecordAddDto()
        {
            Body = "body"
        };

        _mapperMock.Setup(x => x.Map<ApproveRecord>(addObject))
            .Returns(new ApproveRecord() { Body = addObject.Body });
        _approvalServiceMock
            .Setup(x => x.GetApproveRecordByUserIdAsync(correctSub))
            .ReturnsAsync(new ApproveRecord() { Status = ApproveStatus.Rejected });
        _approvalServiceMock
            .Setup(x => x.AddApproveRecordAsync(It.IsAny<ApproveRecord>()))
            .ReturnsAsync(true);

        //Action
        var approveController = new ApproveController(
            _loggerMock.Object,
            _approvalServiceMock.Object,
            _identityClientMock.Object,
            _mapperMock.Object);

        approveController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() { User = GetExistUserClaimsPrincipal() }
        };

        var actionResult = await approveController.AddApproveApplyAsync(addObject);

        //Assert
        Assert.Equal(((OkResult)actionResult).StatusCode, (int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task Add_user_approve_badRequest_when_addDtoNull()
    {
        //Arrange
        ApproveRecordAddDto addObject = null;

        //Action
        var approveController = new ApproveController(
            _loggerMock.Object,
            _approvalServiceMock.Object,
            _identityClientMock.Object,
            _mapperMock.Object);

        var actionResult = await approveController.AddApproveApplyAsync(addObject);

        //Assert
        Assert.Equal(((BadRequestResult)actionResult).StatusCode, (int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Add_user_approve_exception_when_add_fail()
    {
        //Arrange
        var addObject = new ApproveRecordAddDto()
        {
            Body = "body"
        };

        _mapperMock.Setup(x => x.Map<ApproveRecord>(addObject))
            .Returns(new ApproveRecord() { Body = addObject.Body });
        _approvalServiceMock
            .Setup(x => x.AddApproveRecordAsync(It.IsAny<ApproveRecord>()))
            .ReturnsAsync(false);

        //Action
        var approveController = new ApproveController(
            _loggerMock.Object,
            _approvalServiceMock.Object,
            _identityClientMock.Object,
            _mapperMock.Object);

        approveController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() { User = GetExistUserClaimsPrincipal() }
        };
        var asyncAction = new Func<Task>(async () => await approveController.AddApproveApplyAsync(addObject));

        //Assert
        await Assert.ThrowsAsync<BackManageDomainException>(asyncAction);
    }

    [Fact]
    public async Task Add_user_approve_badRequest_when_status_reject()
    {
        //Arrange
        var correctSub = "correctSub";
        var addObject = new ApproveRecordAddDto()
        {
            Body = "body"
        };

        _mapperMock.Setup(x => x.Map<ApproveRecord>(addObject))
            .Returns(new ApproveRecord() { Body = addObject.Body });
        _approvalServiceMock
            .Setup(x => x.GetApproveRecordByUserIdAsync(correctSub))
            .ReturnsAsync(new ApproveRecord() { Status = ApproveStatus.Progressing });

        //Action
        var approveController = new ApproveController(
            _loggerMock.Object,
            _approvalServiceMock.Object,
            _identityClientMock.Object,
            _mapperMock.Object);

        approveController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() { User = GetExistUserClaimsPrincipal() }
        };

        var actionResult = await approveController.AddApproveApplyAsync(addObject);

        //Assert
        Assert.Equal(((BadRequestResult)actionResult).StatusCode, (int)HttpStatusCode.BadRequest);
    }

    private List<ApproveRecord> GetUserApproveFakeList()
    {
        return new List<ApproveRecord>()
        {
            new ApproveRecord()
            {
                Id = 1,
                ApproveUser = "fakeUser",
                Body = "fake body message",
                Status = ApproveStatus.Progressing,
                ApplyTime = DateTime.Now.ToLocalTime()
            },
            new ApproveRecord()
            {
                Id = 2,
                ApproveUser = "fakeUserMale",
                Body = "fake body message 2",
                Status = ApproveStatus.Progressing,
                ApplyTime = DateTime.Now.ToLocalTime()
            }
        };
    }

    private ApproveRecord GetUserApproveFakeItem()
    {
        return new ApproveRecord()
        {
            Id = 100,
            ApproveUser = "fakeUserItem",
            Body = "fake body message Item",
            Status = ApproveStatus.Progressing,
            ApplyTime = DateTime.Now.ToLocalTime()
        };
    }

    private ApproveRecord GetUserApproveNullItem()
    {
        return null;
    }

    private ClaimsPrincipal GetFakeUserClaimsPrincipal()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "fakeId"),
            new Claim(ClaimTypes.Name, "fakeUser"),
            new Claim("sub","can't Found")
        }, "TestAuthentication"));

        return user;
    }

    private ClaimsPrincipal GetExistUserClaimsPrincipal()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "Id"),
            new Claim(ClaimTypes.Name, "User"),
            new Claim("sub","correctSub")
        }, "TestAuthentication"));

        return user;
    }
}