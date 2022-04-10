namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Controllers;

/// <summary>
/// 用户头像上传接口
/// </summary>
[ApiController]
[Route("api/v1/user/avatar")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserImageController : ControllerBase
{
    private readonly ApplicationDbContext _appDbContextService;
    private readonly MinioClient _minioClient;
    private readonly ILogger<UserImageController> _logger;
    private const string _userInfoBucket = "userinfopic";

    public UserImageController(
        ILogger<UserImageController> logger,
        ApplicationDbContext appDbContextService,
        MinioClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _appDbContextService = appDbContextService ?? throw new ArgumentNullException(nameof(appDbContextService));
        _minioClient = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <summary>
    /// 用户——上传用户头像至OSS中
    /// </summary>
    /// <param name="avatar"></param>
    /// <returns></returns>
    [HttpPost("oss")]
    [Authorize]
    public async Task<IActionResult> PostAvatarToOssAsync([FromForm] IFormFile avatar)
    {
        var userId = User.FindFirstValue("sub");

        //检查用户的正确性
        var user = await _appDbContextService.Users.FindAsync(userId);
        if (user == null || user.Id != User.FindFirstValue("sub"))
            return BadRequest();

        if (avatar == null || avatar.Length == 0 || avatar.FileName.Length >= 30)
            return BadRequest("avatar file empty or file name > 30 char");

        var acceptTypes = new[] { ".jpg", ".jpeg", ".png" };
        if (acceptTypes.All(t => t != Path.GetExtension(avatar.FileName)?.ToLower()))
            return BadRequest("File type not valid, only jpg and png are acceptable.");

        if (avatar.Length > 10 * 1024 * 3 * 1000) return BadRequest("File size cannot exceed 3M");

        //判断bucket是否存在
        var bucketExist = await _minioClient.BucketExistsAsync(_userInfoBucket);
        if (!bucketExist)
        {
            await _minioClient.MakeBucketAsync(_userInfoBucket);
            _logger.LogInformation("Minio OSS create a bucket: {BucketName}", _userInfoBucket);
        }

        var avatarName = "avatar-" + DateTime.Now.Ticks + Path.GetExtension(avatar.FileName);
        var uploadPath = $"/{userId}/{avatarName}";

        await using var stream = avatar.OpenReadStream();
        await _minioClient.PutObjectAsync(_userInfoBucket,
            uploadPath,
            stream,
            avatar.Length,
            avatar.ContentType);
        _logger.LogInformation("User avatar uploads successful -> bucket: {BucketName}, object:{ObjectName}", _userInfoBucket, uploadPath);

        ////删除修改前的头像
        //var oldAvatar = user.Avatar;
        //await _minioClient.RemoveObjectAsync(_bucket, oldAvatar);

        //更新用户头像引用地址
        user.Avatar = $"{_userInfoBucket}{uploadPath}";
        await _appDbContextService.SaveChangesAsync();

        return Ok();
    }
}