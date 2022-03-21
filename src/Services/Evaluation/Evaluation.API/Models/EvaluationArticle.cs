namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models;

public class EvaluationArticle
{
    public int ArticleId { get; set; }

    /// <summary>
    ///     作者Id
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    ///     作者姓名
    /// </summary>
    public string NickName { get; set; }

    /// <summary>
    ///     标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     略缩图
    /// </summary>
    public string? DescriptionImage { get; set; }

    /// <summary>
    ///     详情图
    /// </summary>
    public string? ArticleImage { get; set; }

    /// <summary>
    ///     内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    ///     创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    ///     更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    ///     访问量
    /// </summary>
    public int JoinCount { get; set; }

    /// <summary>
    ///     点赞数量
    /// </summary>
    public int SupportCount { get; set; }

    /// <summary>
    ///     逻辑删除
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    ///     测评描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     测评状态
    /// </summary>
    public ArticleStatus Status { get; set; }

    /// <summary>
    ///     测评类别Id
    /// </summary>
    public int? CategoryTypeId { get; set; }

    /// <summary>
    ///     测评类别导航属性
    /// </summary>
    [JsonIgnore]
    public EvaluationCategory CategoryType { get; set; }

    /// <summary>
    ///     关联游戏外键
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    ///     关联游戏名
    /// </summary>
    public string GameName { get; set; }

    /// <summary>
    ///     测评的评论
    /// </summary>
    [JsonIgnore]
    public List<EvaluationComment> EvaluationComments { get; set; }
}

public enum ArticleStatus
{
    /// <summary>
    ///     发布状态
    /// </summary>
    Normal = 0,

    /// <summary>
    ///     草稿状态
    /// </summary>
    Draft = 1,

    /// <summary>
    ///     异常状态
    /// </summary>
    Wrong = 2
}

//public static class EvaluationArticleExtensions
//{
//    /// <summary>
//    ///     当测评人员添加测评时未选择描述图片和文章图片
//    ///     使用默认图片填充
//    /// </summary>
//    /// <param name="article"></param>
//    /// <param name="articlePicBaseUrl"></param>
//    /// <param name="descriptionPicBaseUrl"></param>
//    public static void FillDefaultArticlePicture(this EvaluationArticle article, string articlePicBaseUrl,
//        string descriptionPicBaseUrl)
//    {
//        if (article != null)
//        {
//            if (string.IsNullOrEmpty(article.DescriptionImage)) article.DescriptionImage = descriptionPicBaseUrl;

//            if (string.IsNullOrEmpty(article.ArticleImage)) article.ArticleImage = articlePicBaseUrl;
//        }
//    }
//}