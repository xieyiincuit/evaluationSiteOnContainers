namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.DtoModels;

public class ArticleAdminDto
{
    public int Id { get; set; }
    public string CategoryName { get; set; }
    public string GameName { get; set; }
    public string AuthorName { get; set; }
    public string Title { get; set; }
    public DateTime CreateTime { get; set; }
    public int CommentCounts { get; set; }
    public int ViewCounts { get; set; }
    public int LikeCounts { get; set; }
}