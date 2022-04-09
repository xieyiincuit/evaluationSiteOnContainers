namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Controllers;

/// <summary>
/// 测评服务图片上传接口
/// </summary>
[ApiController]
[Route("api/v1")]
public class EvaluationImageController : ControllerBase
{
    private readonly MinioClient _minioClient;
    private readonly ILogger<EvaluationImageController> _logger;
    private const string _articleBucket = "articleinfo";
    private const string _picUploadRole = "evaluator";

    public EvaluationImageController(ILogger<EvaluationImageController> logger, MinioClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _minioClient = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <summary>
    /// 测评人员——上传文章Banner
    /// </summary>
    /// <param name="file">格式: jpg, jpeg, png. 大小:3M以下. 文件名: 30个字符以内</param>
    /// <returns>图片存储Url 访问图片通过http:localhost:9000/url</returns>
    [HttpPost("pic")]
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

        // 判断Minio bucket是否存在
        var bucketExist = await _minioClient.BucketExistsAsync(_articleBucket);
        if (!bucketExist)
        {
            // 不存在则新建Bucket
            await _minioClient.MakeBucketAsync(_articleBucket);
            _logger.LogInformation("Minio OSS create a bucket: {BucketName}", _articleBucket);
        }

        // 构建图片名
        var articlePic = "article-" + DateTime.Now.Ticks + Path.GetExtension(file.FileName);
        // 获取图片上传地址
        var uploadFile = $"/evaluation/{articlePic}";
        await using var stream = file.OpenReadStream();
        // 上传至OSS
        await _minioClient.PutObjectAsync(_articleBucket,
            uploadFile,
            stream,
            file.Length,
            file.ContentType);
        _logger.LogInformation("article Pic uploads successful -> bucket: {BucketName}, object:{ObjectName}", _articleBucket, uploadFile);

        //返回图片地址
        var sourcePath = $"{_articleBucket}{uploadFile}";
        return Ok(sourcePath);
    }
}