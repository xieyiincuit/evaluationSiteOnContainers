namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.DtoModels;

public class ArticleCommentDto
{
    public int CommentId { get; set; }
    public string UserId { get; set; }
    public string Content { get; set; }
    public int SupportCount { get; set; }
    public DateTime CreateTime { get; set; }
    public int RepliesCount { get; set; }

    //Prevent null exception
    public List<ReplyCommentDto> Replies { get; set; } = new();
}