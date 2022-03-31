namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.DtoModels;
public class ArticleGameDto
{
    public int ArticleId { get; set; }
    public string AuthorId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public int CommentsCount { get; set; }
    public int SupportCount { get; set; }
    public DateTime CreateTime { get; set; }
}
