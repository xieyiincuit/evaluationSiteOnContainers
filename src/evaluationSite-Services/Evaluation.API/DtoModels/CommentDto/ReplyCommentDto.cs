namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.DtoModels;

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
    ///     回复的评论Id
    /// </summary>
    public int ReplyCommentId { get; set; }

    /// <summary>
    ///     回复的用户名
    /// </summary>
    public string ReplyNickName { get; set; }

    /// <summary>
    ///     每个子评论所属于的唯一父评论
    /// </summary>
    public int RootCommentId { get; set; }
}