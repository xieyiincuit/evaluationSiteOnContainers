namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Model;

public class EvaluationComment
{
    public int CommentId { get; set; }

    public string Content { get; set; }

    public int SupportCount { get; set; }

    public int UserId { get; set; }

    public string NickName { get; set; }

    public string? Avatar { get; set; }

    public DateTime CreateTime { get; set; }

    public bool? IsDeleted { get; set; }

    /// <summary>
    /// 是否为回复
    /// </summary>
    public bool? IsReplay { get; set; }

    /// <summary>
    /// 回复的评论Id
    /// </summary>
    public int? ReplayCommentId { get; set; }

    /// <summary>
    /// 回复的用户Id
    /// </summary>
    public int ReplyUserId { get; set; }

    /// <summary>
    /// 回复的用户名
    /// </summary>
    public string ReplyNickName { get; set; }

    /// <summary>
    /// 关联测评的外键
    /// </summary>
    public int ArticleId { get; set; }

    /// <summary>
    /// 测评导航属性
    /// </summary>
    public EvaluationArticle EvaluationArticle { get; set; }
}