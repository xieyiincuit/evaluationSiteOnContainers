namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Model;

public class EvaluationComment
{
    public int Id { get; set; }

    public string Content { get; set; }

    public int LikeNums { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; }

    public string? UserAvatar { get; set; }

    public DateTime CreateTime { get; set; }

    public bool? IsDeleted { get; set; }

    /// <summary>
    /// 是否为回复
    /// </summary>
    public bool? IsReplay { get; set; }

    /// <summary>
    /// 回复的评论Id
    /// </summary>
    public int? ReplayId { get; set; }

    /// <summary>
    /// 关联测评的外键
    /// </summary>
    public int ArticleId { get; set; }

    /// <summary>
    /// 测评导航属性
    /// </summary>
    public EvaluationArticle EvaluationArticle { get; set; }
}