namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

/// <summary>
/// 游戏资料服务图片上传接口
/// </summary>
[ApiController]
[Route("api/v1")]
public class GameImageController : ControllerBase
{
    private readonly MinioClient _minioClient;
    private readonly ILogger<GameImageController> _logger;
    private const string _gameInfoBucket = "gameinfopic";
    private const string _shopInfoBucket = "shopinfopic";
    private const string _picUploadRole = "administrator";
    public GameImageController(ILogger<GameImageController> logger, MinioClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _minioClient = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <summary>
    /// 管理员——上传游戏略缩图和详细图
    /// </summary>
    /// <param name="file">格式: jpg, jpeg, png. 大小:3M以下. 文件名: 30个字符以内</param>
    /// <returns>图片存储Url 访问图片通过http:localhost:9000/url</returns>
    [HttpPost("game/pic")]
    [Authorize(Roles = _picUploadRole)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(String), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PostGamePicToOssAsync([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0 || file.FileName.Length >= 30)
            return BadRequest("file empty or file name > 30 char");

        var acceptTypes = new[] { ".jpg", ".jpeg", ".png" };
        if (acceptTypes.All(t => t != Path.GetExtension(file.FileName).ToLower()))
            return BadRequest("File type not valid, only jpg and png are acceptable.");

        if (file.Length > 10 * 1024 * 3 * 1000) return BadRequest("File size cannot exceed 3M");

        //判断bucket是否存在
        var bucketExist = await _minioClient.BucketExistsAsync(_gameInfoBucket);
        if (!bucketExist)
        {
            await _minioClient.MakeBucketAsync(_gameInfoBucket);
            _logger.LogInformation("Minio OSS create a bucket: {BucketName}", _gameInfoBucket);
        }

        var gamePic = "gamePic-" + DateTime.Now.Ticks + Path.GetExtension(file.FileName);
        var uploadFile = $"/gamerepo/{gamePic}";
        await using var stream = file.OpenReadStream();
        await _minioClient.PutObjectAsync(_gameInfoBucket,
            uploadFile,
            stream,
            file.Length,
            file.ContentType);
        _logger.LogInformation("game Pic uploads successful -> bucket: {BucketName}, object:{ObjectName}", _gameInfoBucket, uploadFile);

        var sourcePath = $"{_gameInfoBucket}{uploadFile}";
        return Ok(sourcePath);
    }

    /// <summary>
    /// 管理员——上传商品图片
    /// </summary>
    /// <param name="file">格式: jpg, jpeg, png. 大小:3M以下. 文件名: 30个字符以内</param>
    /// <returns>图片存储Url 访问图片通过http:localhost:9000/url</returns>
    [HttpPost("shop/pic")]
    [Authorize(Roles = _picUploadRole)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(String), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PostShopPicToOssAsync([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0 || file.FileName.Length >= 30)
            return BadRequest("file empty or file name > 30 char");

        var acceptTypes = new[] { ".jpg", ".jpeg", ".png" };
        if (acceptTypes.All(t => t != Path.GetExtension(file.FileName).ToLower()))
            return BadRequest("File type not valid, only jpg and png are acceptable.");

        if (file.Length > 10 * 1024 * 3 * 1000) return BadRequest("File size cannot exceed 3M");

        var bucketExist = await _minioClient.BucketExistsAsync(_shopInfoBucket);
        if (!bucketExist)
        {
            await _minioClient.MakeBucketAsync(_shopInfoBucket);
            _logger.LogInformation("Minio OSS create a bucket: {BucketName}", _shopInfoBucket);
        }

        var shopPic = "shopPic-" + DateTime.Now.Ticks + Path.GetExtension(file.FileName);
        var uploadFile = $"/gamerepo/{shopPic}";
        await using var stream = file.OpenReadStream();
        await _minioClient.PutObjectAsync(_shopInfoBucket,
            uploadFile,
            stream,
            file.Length,
            file.ContentType);
        _logger.LogInformation("shopPic Pic uploads successful -> bucket: {BucketName}, object:{ObjectName}", _shopInfoBucket, uploadFile);

        var sourcePath = $"{_shopInfoBucket}{uploadFile}";
        return Ok(sourcePath);
    }
}