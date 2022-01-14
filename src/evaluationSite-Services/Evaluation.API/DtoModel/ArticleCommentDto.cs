namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.DtoModel;

public class ArticleCommentDto
{
    public int CommentId { get; set; }
    public int UserId { get; set; }
    public string NickName { get; set; }
    public string? Avatar { get; set; }
    public string Content { get; set; }
    public int SupportCount { get; set; }
    public DateTime CreateTime { get; set; }
    public int RepliesCount { get; set; }

    public List<ReplyCommentDto> Replies { get; set; }
}

public class ReplyCommentDto
{
    public int CommentId { get; set; }
    public int UserId { get; set; }
    public string NickName { get; set; }
    public string? Avatar { get; set; }
    public string ReplyContent { get; set; }
    public int SupportCount { get; set; }
    public DateTime CreateTime { get; set; }

    public List<ReplyCommentDto> Replies { get; set; }
    public int ReplyCommentId { get; set; }
    public string ReplyNickName { get; set; }
}
