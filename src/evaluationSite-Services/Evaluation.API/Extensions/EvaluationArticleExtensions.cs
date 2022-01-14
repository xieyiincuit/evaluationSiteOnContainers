namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Extensions;

public static class EvaluationArticleExtensions
{
    /// <summary>
    /// 当测评人员添加测评时未选择描述图片和文章图片
    /// 使用默认图片填充
    /// </summary>
    /// <param name="article"></param>
    /// <param name="articlePicBaseUrl"></param>
    /// <param name="descriptionPicBaseUrl"></param>
    public static void FillDefaultArticlePicture(this EvaluationArticle article, string articlePicBaseUrl, string descriptionPicBaseUrl)
    {
        if (article != null)
        {
            if (string.IsNullOrEmpty(article.DesciptionImage))
            {
                article.DesciptionImage = descriptionPicBaseUrl;
            }

            if (string.IsNullOrEmpty(article.ArticleImage))
            {
                article.ArticleImage = articlePicBaseUrl;
            }
        }
    }
}
