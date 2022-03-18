namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Controllers;

[ApiController]
[Route("api/v1/user/avatar")]
public class PostImageController : ControllerBase
{
    private readonly ApplicationDbContext _appDbContextService;
    private readonly MinioClient _minioClient;
    private readonly ILogger<PostImageController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private const string _bucket = "userinfopic";

    public PostImageController(
        ILogger<PostImageController> logger,
        IWebHostEnvironment webHostEnvironment,
        ApplicationDbContext appDbContextService,
        MinioClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        _appDbContextService = appDbContextService ?? throw new ArgumentNullException(nameof(appDbContextService));
        _minioClient = client ?? throw new ArgumentNullException(nameof(client));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostAvatarAsync([FromForm] IFormFile avatar)
    {
        var userId = User.FindFirstValue("sub");
        //检查用户的正确性
        var user = await _appDbContextService.Users.FindAsync(userId);
        if (user == null || user.Id != User.FindFirstValue("sub"))
            return BadRequest();

        if (avatar == null || avatar.Length == 0 || avatar.FileName.Length >= 50)
            return BadRequest("avatar file empty or file name > 50 char");

        var acceptTypes = new[] { ".jpg", ".jpeg", ".png" };
        if (acceptTypes.All(t => t != Path.GetExtension(avatar.FileName)?.ToLower()))
            return BadRequest("File type not valid, only jpg and png are acceptable.");

        if (avatar.Length > 10 * 1024 * 3 * 1000) return BadRequest("File size cannot exceed 3M");

        if (string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath))
            _webHostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolderPath))
            Directory.CreateDirectory(uploadsFolderPath);

        _logger.LogDebug("---- user:{userName} wanna post a file: upLoadDirectory:{path}", user.NickName,
            uploadsFolderPath);

        var fileName = user.NickName + "-avatar-" + DateTime.Now.Millisecond + Path.GetExtension(avatar.FileName);
        var fileSavePath = Path.Combine(uploadsFolderPath, fileName);
        _logger.LogDebug("---- user:{userName} save a avatar: savePath:{path}, fileName:{name}", user.NickName,
            fileSavePath, fileName);

        //保存文件到文件资源库
        await using var stream = new FileStream(fileSavePath, FileMode.Create);
        await avatar.CopyToAsync(stream);
        _logger.LogDebug("---- user:{userName} save a avatar successfully", user.NickName);

        //删除旧头像文件
        var oldUserAvatar = user.Avatar;
        var fileDeletePath = Path.Combine(uploadsFolderPath, oldUserAvatar);
        if (System.IO.File.Exists(fileDeletePath)) System.IO.File.Delete(fileDeletePath);
        _logger.LogDebug("---- delete user:{userName} old avatar:{oldPath}", user.NickName, fileDeletePath);

        //更新用户头像引用地址
        user.Avatar = fileName;
        await _appDbContextService.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("oss")]
    [Authorize]
    public async Task<IActionResult> PostAvatarToOSSAsync([FromForm] IFormFile avatar)
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
        var bucketExist = await _minioClient.BucketExistsAsync(_bucket);
        if (!bucketExist)
        {
            await _minioClient.MakeBucketAsync(_bucket);
            _logger.LogInformation("Minio OSS create a bucket: {BucketName}", _bucket);
        }

        var avatarName = user.NickName + "-avatar-" + DateTime.Now.Minute + DateTime.Now.Millisecond + Path.GetExtension(avatar.FileName);

        await using var stream = avatar.OpenReadStream();
        await _minioClient.PutObjectAsync(_bucket,
                                 avatarName,
                                 stream,
                                 avatar.Length,
                                 avatar.ContentType);
        _logger.LogInformation("User avatar uploads successful -> bucket: {BucketName}, object:{ObjectName}", _bucket, avatarName);

        var oldAvatar = user.Avatar;
        //更新用户头像引用地址
        user.Avatar = avatarName;
        await _appDbContextService.SaveChangesAsync();
        await _minioClient.RemoveObjectAsync(_bucket, oldAvatar);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("{uid}")]
    public async Task<IActionResult> GetCurrentUserAvatarAsync([FromRoute] string uid)
    {
        if (uid != User.FindFirstValue("sub")) return BadRequest();
        var user = await _appDbContextService.Users
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == uid);

        if (user == null) return BadRequest();

        var result = new UserAvatarDto { Id = user.Id, NickName = user.NickName, Avatar = user.Avatar };

        return Ok(result);
    }

    [HttpPost("batch")]
    [AllowAnonymous]
    public async Task<IActionResult> BatchGetAvatarAsync([FromBody] List<string> userIds)
    {
        if (userIds == null || userIds.Count == 0 || userIds.Count > 5)
            return BadRequest("batch get avatar, id count should be 1-5");
        var result = new List<UserAvatarDto>();
        var errorList = new List<string>();

        foreach (var id in userIds)
        {
            var user = await _appDbContextService.Users
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new UserAvatarDto { Id = x.Id, NickName = x.NickName, Avatar = x.Avatar })
                .FirstOrDefaultAsync();
            if (user != null)
                result.Add(user);
            else
                errorList.Add(id);
        }

        if (errorList.Any())
            _logger.LogWarning("---- GetUserInfo Error userIds:{@ids}", errorList);

        if (!result.Any()) return NotFound();
        return Ok(result);
    }
}