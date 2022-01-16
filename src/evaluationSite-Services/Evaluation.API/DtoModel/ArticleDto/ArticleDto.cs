namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.DtoModel;

public class ArticleDto
{
    public int ArticleId { get; set; }

    public int UserId { get; set; }

    public string Author { get; set; }

    public string Title { get; set; }

    public string? DesciptionImage { get; set; }

    public string? ArticleImage { get; set; }

    public string Content { get; set; }

    public DateTime CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    public int JoinCount { get; set; }

    public int CommentsCount { get; set; }

    public int SupportCount { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Description { get; set; }

    public ArticleStatus Status { get; set; }

    public int CategoryTypeId { get; set; }

    public int GameId { get; set; }

    public string GameName { get; set; }
}
