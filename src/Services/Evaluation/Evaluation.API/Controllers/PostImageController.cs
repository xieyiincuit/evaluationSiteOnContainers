namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Controllers;

[ApiController]
[Route("api/v1")]
public class PostImageController : ControllerBase
{
    private readonly MinioClient _minioClient;
    private readonly ILogger<PostImageController> _logger;
    private const string _articleBucket = "articleinfo";

    public PostImageController(ILogger<PostImageController> logger, MinioClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _minioClient = client ?? throw new ArgumentNullException(nameof(client));
    }

    [HttpPost("pic")]
    [Authorize]
    public async Task<IActionResult> PostGamePicToOssAsync([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0 || file.FileName.Length >= 30)
            return BadRequest("file empty or file name > 30 char");

        var acceptTypes = new[] { ".jpg", ".jpeg", ".png" };
        if (acceptTypes.All(t => t != Path.GetExtension(file.FileName).ToLower()))
            return BadRequest("File type not valid, only jpg and png are acceptable.");

        if (file.Length > 10 * 1024 * 3 * 1000) return BadRequest("File size cannot exceed 3M");

        //判断bucket是否存在
        var bucketExist = await _minioClient.BucketExistsAsync(_articleBucket);
        if (!bucketExist)
        {
            await _minioClient.MakeBucketAsync(_articleBucket);
            _logger.LogInformation("Minio OSS create a bucket: {BucketName}", _articleBucket);
        }

        var articlePic = "article-" + DateTime.Now.Ticks + Path.GetExtension(file.FileName);
        var uploadFile = $"/evaluation/{articlePic}";
        await using var stream = file.OpenReadStream();
        await _minioClient.PutObjectAsync(_articleBucket,
            uploadFile,
            stream,
            file.Length,
            file.ContentType);
        _logger.LogInformation("article Pic uploads successful -> bucket: {BucketName}, object:{ObjectName}", _articleBucket, uploadFile);

        var sourcePath = $"{_articleBucket}{uploadFile}";
        return Ok(sourcePath);
    }
}