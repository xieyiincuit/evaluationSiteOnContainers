namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Model;

public class EvaluationArticle
{    
    public int Id { get; set; }
  
    public string Author { get; set; }

    public string Title { get; set; }

    public string? DesciptionImage { get; set; }
    public string? ArticleImage { get; set; }

    public string Content { get; set; }

    public DateTime CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 访问量
    /// </summary>
    public int Traffic { get; set; }

    /// <summary>
    /// 评论数量
    /// </summary>
    public int CommentNums { get; set; }

    /// <summary>
    /// 点赞数量
    /// </summary>
    public int LikeNums { get; set; }

    /// <summary>
    /// 逻辑删除
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// 测评描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 测评状态
    /// </summary>
    public ArticleStatus Status { get; set; }

    /// <summary>
    /// 类别Id
    /// </summary>
    public int CategoryTypeId { get; set; }

    /// <summary>
    /// 类别导航属性
    /// </summary>
    public EvaluationCategory CategoryType { get; set; }

    /// <summary>
    /// 关联游戏外键
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// 关联游戏名
    /// </summary>
    public string GameName { get; set; }

    /// <summary>
    /// 测评的评论
    /// </summary>
    public List<EvaluationComment> EvaluationComments { get; set; }
}

public enum ArticleStatus
{
    /// <summary>
    /// 发布状态
    /// </summary>
    Normal = 0,

    /// <summary>
    /// 草稿状态
    /// </summary>
    Draft = 1,

    /// <summary>
    /// 异常状态
    /// </summary>
    Wrong = 2
}