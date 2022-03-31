namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models;

public class EvaluationComment
{
    public int CommentId { get; set; }

    public string Content { get; set; }

    public int SupportCount { get; set; }

    public string UserId { get; set; }

    public DateTime CreateTime { get; set; }

    public bool? IsDeleted { get; set; }

    /// <summary>
    ///     是否为回复
    /// </summary>
    public bool? IsReply { get; set; }

    /// <summary>
    ///     回复的评论Id
    /// </summary>
    public int? ReplyCommentId { get; set; }

    /// <summary>
    ///     回复的用户Id
    /// </summary>
    public string? ReplyUserId { get; set; }

    /// <summary>
    ///     回复评论所属于哪个主评论
    /// </summary>
    public int? RootCommentId { get; set; }

    /// <summary>
    ///     关联测评的外键
    /// </summary>
    public int ArticleId { get; set; }

    /// <summary>
    ///     测评导航属性
    /// </summary>
    public EvaluationArticle EvaluationArticle { get; set; }
}