﻿namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Services;

public class EvaluationArticleService : IEvaluationArticle
{
    private readonly EvaluationContext _evaluationContext;
    private readonly EvaluationSettings _settings;

    public EvaluationArticleService(EvaluationContext context, IOptionsSnapshot<EvaluationSettings> settings)
    {
        _evaluationContext = context;
        _settings = settings.Value;
    }

    public async Task<long> CountArticlesAsync() => await _evaluationContext.Articles.LongCountAsync();

    public async Task<long> CountArticlesByTypeAsync(int categoryId) => await _evaluationContext.Articles.Where(x=>x.CategoryTypeId == categoryId).LongCountAsync();

    public async Task<List<EvaluationArticle>> GetArticlesAsync(int pageSize, int pageIndex, string ids = null)
    {
        var articles = new List<EvaluationArticle>();

        if (!string.IsNullOrEmpty(ids))
        {
            articles = await GetArticlesByIdsAsync(ids);
        }
        else
        {
            articles = await _evaluationContext.Articles
                .AsNoTracking()
                .OrderBy(c => c.CreateTime)
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize)
                .ToListAsync();

            //兼容图片显示
            articles = ChangePicsUri(articles);
        }

        return articles;
    }

    public async Task<EvaluationArticle> GetArticleAsync(int id)
    {
        var article = await _evaluationContext.Articles.FindAsync(id);

        if (article != null)
        {
            var articlePicBaseUrl = _settings.ArticlePicBaseUrl;
            var descriptionPicBaseUrl = _settings.DescriptionPicBaseUrl;
            article.FillDefaultArticlePicture(articlePicBaseUrl, descriptionPicBaseUrl);
        }

        return article;
    }

    public async Task<List<EvaluationArticle>> GetArticlesAsync(int pageSize, int pageIndex, int categoryTypeId)
    {
        var articles = await _evaluationContext.Articles
                .Where(art => art.CategoryTypeId == categoryTypeId)
                .OrderBy(c => c.CreateTime)
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

        //兼容图片显示
        articles = ChangePicsUri(articles);

        return articles;
    }

    public async Task<bool> IsArticleExist(int id)
    {
        var article = await _evaluationContext.Articles.FindAsync(id);
        return article != null;
    }

    /// <summary>
    /// Bulks get articles
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    private async Task<List<EvaluationArticle>> GetArticlesByIdsAsync(string ids)
    {
        var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));

        if (!numIds.All(nid => nid.Ok))
        {
            return new List<EvaluationArticle>();
        }

        var idsToSelect = numIds.Select(id => id.Value);
        var items = await _evaluationContext.Articles.AsNoTracking().Where(ci => idsToSelect.Contains(ci.ArticleId)).ToListAsync();

        //检查pics设置
        items = ChangePicsUri(items);

        return items;
    }

    /// <summary>
    /// Fill picture use default
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    private List<EvaluationArticle> ChangePicsUri(List<EvaluationArticle> items)
    {
        var articlePicBaseUrl = _settings.ArticlePicBaseUrl;
        var descriptionPicBaseUrl = _settings.DescriptionPicBaseUrl;

        foreach (var item in items)
        {
            item.FillDefaultArticlePicture(articlePicBaseUrl, descriptionPicBaseUrl);
        }

        return items;
    }
}