namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Controllers;

[ApiController]
[Route("api/v1/game")]
public class PostImageController : ControllerBase
{
    private readonly MinioClient _minioClient;
    private readonly ILogger<PostImageController> _logger;
    private const string _bucket = "gameinfopic";

    public PostImageController(
        ILogger<PostImageController> logger,
        IWebHostEnvironment webHostEnvironment,
        MinioClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _minioClient = client ?? throw new ArgumentNullException(nameof(client));
    }

    [HttpPost("pic")]
    [Authorize]
    public async Task<IActionResult> PostToOSSAsync([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0 || file.FileName.Length >= 30)
            return BadRequest("file empty or file name > 30 char");

        var acceptTypes = new[] { ".jpg", ".jpeg", ".png" };
        if (acceptTypes.All(t => t != Path.GetExtension(file.FileName)?.ToLower()))
            return BadRequest("File type not valid, only jpg and png are acceptable.");

        if (file.Length > 10 * 1024 * 3 * 1000) return BadRequest("File size cannot exceed 3M");

        //判断bucket是否存在
        var bucketExist = await _minioClient.BucketExistsAsync(_bucket);
        if (!bucketExist)
        {
            await _minioClient.MakeBucketAsync(_bucket);
            _logger.LogInformation("Minio OSS create a bucket: {BucketName}", _bucket);
        }

        var gamePic = "gamePic-" + DateTime.Now.Minute + DateTime.Now.Millisecond + Path.GetExtension(file.FileName);

        await using var stream = file.OpenReadStream();
        await _minioClient.PutObjectAsync(_bucket,
                                 gamePic,
                                 stream,
                                 gamePic.Length,
                                 file.ContentType);
        _logger.LogInformation("game Pic uploads successful -> bucket: {BucketName}, object:{ObjectName}", _bucket, gamePic);
        return NoContent();
    }
}