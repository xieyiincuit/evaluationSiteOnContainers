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

    //Prevent null exception
    public List<ReplyCommentDto> Replies { get; set; } = new List<ReplyCommentDto>();
}

public class ReplyCommentDto
{
    public int CommentId { get; set; }
    public int UserId { get; set; }
    public string NickName { get; set; }
    public string? Avatar { get; set; }
    public string Content { get; set; }
    public int SupportCount { get; set; }
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 回复的评论Id
    /// </summary>
    public int ReplyCommentId { get; set; }

    /// <summary>
    /// 回复的用户名
    /// </summary>
    public string ReplyNickName { get; set; }

    /// <summary>
    /// 每个子评论所属于的唯一父评论
    /// </summary>
    public int RootCommentId { get; set; }
}
