namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Services;

public class EvaluationArticleService : IEvaluationArticleService
{
    private readonly EvaluationContext _evaluationContext;
    private readonly EvaluationSettings _settings;

    public EvaluationArticleService(EvaluationContext context, IOptionsSnapshot<EvaluationSettings> settings)
    {
        _evaluationContext = context;
        _settings = settings.Value;
    }

    public async Task<int> CountArticlesAsync()
    {
        return await _evaluationContext.Articles.CountAsync();
    }

    public async Task<int> CountArticlesByTypeAsync(int categoryId)
    {
        return await _evaluationContext.Articles.Where(x => x.CategoryTypeId == categoryId).CountAsync();
    }

    public async Task<List<EvaluationArticle>> GetArticlesAsync(int pageSize, int pageIndex, string ids = null)
    {
        List<EvaluationArticle> articles;

        if (!string.IsNullOrEmpty(ids))
        {
            articles = await GetArticlesByIdsAsync(ids);
        }
        else
        {
            articles = await _evaluationContext.Articles
                .OrderBy(c => c.CreateTime)
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            //兼容图片显示
            articles = ChangePicsUri(articles);
        }

        return articles;
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

    public async Task<EvaluationArticle> GetArticleAsync(int id)
    {
        var article = await _evaluationContext.Articles.AsNoTracking().FirstOrDefaultAsync(x => x.ArticleId == id);

        if (article != null)
        {
            var articlePicBaseUrl = _settings.ArticlePicBaseUrl;
            var descriptionPicBaseUrl = _settings.DescriptionPicBaseUrl;
            article.FillDefaultArticlePicture(articlePicBaseUrl, descriptionPicBaseUrl);
        }

        return article;
    }

    public async Task<bool> IsArticleExist(int id)
    {
        var article = await _evaluationContext.Articles.AsNoTracking().FirstOrDefaultAsync(x => x.ArticleId == id);
        return article != null;
    }

    public async Task<bool> AddArticleAsync(EvaluationArticle evaluationArticle)
    {
        evaluationArticle.CreateTime = DateTime.Now.ToLocalTime();
        await _evaluationContext.Articles.AddAsync(evaluationArticle);
        return await _evaluationContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteArticleAsync(int id)
    {
        var deleteEntity = await _evaluationContext.Articles.FindAsync(id);
        if (deleteEntity != null)
        {
            _evaluationContext.Articles.Remove(deleteEntity);
            if (await _evaluationContext.SaveChangesAsync() > 0)
                return true;
        }

        return false;
    }

    public async Task<bool> UpdateArticleAsync(EvaluationArticle article)
    {
        article.UpdateTime = DateTime.Now.ToLocalTime();
        _evaluationContext.Articles.Update(article);
        return await _evaluationContext.SaveChangesAsync() > 0;
    }


    public async Task<List<EvaluationArticle>> GetArticlesByGameInfoAsync(int gameId)
    {
        //追踪实体修改游戏名
        var articles = await _evaluationContext.Articles.Where(x => x.GameId == gameId).ToListAsync();
        return articles;
    }

    public async Task<List<EvaluationArticle>> GetArticlesByAuthorInfoAsync(string userId)
    {
        var articles = await _evaluationContext.Articles.Where(x => x.UserId == userId).ToListAsync();
        return articles;
    }

    public async Task<bool> BatchUpdateArticlesAsync()
    {
        return await _evaluationContext.SaveChangesAsync() > 0;
    }

    /// <summary>
    ///     Bulks get articles
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    private async Task<List<EvaluationArticle>> GetArticlesByIdsAsync(string ids)
    {
        var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out var x), Value: x));

        if (!numIds.All(nid => nid.Ok)) return new List<EvaluationArticle>();

        var idsToSelect = numIds.Select(id => id.Value);
        var items = await _evaluationContext.Articles.AsNoTracking()
            .Where(ci => idsToSelect.Contains(ci.ArticleId))
            .ToListAsync();

        //检查pics设置
        items = ChangePicsUri(items);

        return items;
    }

    /// <summary>
    ///     Fill picture use default
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    private List<EvaluationArticle> ChangePicsUri(List<EvaluationArticle> items)
    {
        var articlePicBaseUrl = _settings.ArticlePicBaseUrl;
        var descriptionPicBaseUrl = _settings.DescriptionPicBaseUrl;

        foreach (var item in items) item.FillDefaultArticlePicture(articlePicBaseUrl, descriptionPicBaseUrl);

        return items;
    }
}