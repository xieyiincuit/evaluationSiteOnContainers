namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Services;

public class EvaluationArticleService : IEvaluationArticleService
{
    private readonly EvaluationContext _evaluationContext;

    public EvaluationArticleService(EvaluationContext context)
    {
        _evaluationContext = context;
    }

    public async Task<int> CountArticlesAsync()
    {
        return await _evaluationContext.Articles.CountAsync(x => x.Status == ArticleStatus.Normal);
    }

    public async Task<int> CountArticlesByTypeAsync(int categoryId)
    {
        return await _evaluationContext.Articles.CountAsync(x => x.Status == ArticleStatus.Normal && x.CategoryTypeId == categoryId);
    }

    public async Task<int> CountArticlesByUserAsync(string userId)
    {
        return await _evaluationContext.Articles.CountAsync(x => x.UserId == userId);
    }
    public async Task<int> CountArticlesByGameAsync(int gameId)
    {
        return await _evaluationContext.Articles.CountAsync(x => x.GameId == gameId);
    }
    public async Task<List<ArticleSmallDto>> GetArticlesAsync(int pageSize, int pageIndex)
    {
        var articles = await _evaluationContext.Articles
            .Select(x => new ArticleSmallDto()
            {
                ArticleId = x.ArticleId,
                Author = x.NickName,
                CategoryTypeId = x.CategoryTypeId,
                CreateTime = x.CreateTime,
                Description = x.Description,
                DescriptionImage = x.DescriptionImage,
                GameId = x.GameId,
                GameName = x.GameName,
                Status = x.Status,
                UserId = x.UserId,
                Title = x.Title,
                SupportCount = x.SupportCount,
            })
            .Where(x => x.Status == ArticleStatus.Normal)
            .OrderByDescending(c => c.CreateTime)
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return articles;
    }

    public async Task<List<ArticleSmallDto>> GetArticlesAsync(int pageSize, int pageIndex, int categoryTypeId)
    {
        var articles = await _evaluationContext.Articles
            .Where(art => art.CategoryTypeId == categoryTypeId && art.Status == ArticleStatus.Normal)
            .Select(x => new ArticleSmallDto()
            {
                ArticleId = x.ArticleId,
                Author = x.NickName,
                CategoryTypeId = x.CategoryTypeId,
                CreateTime = x.CreateTime,
                Description = x.Description,
                DescriptionImage = x.DescriptionImage,
                GameId = x.GameId,
                GameName = x.GameName,
                Status = x.Status,
                UserId = x.UserId,
                Title = x.Title,
                SupportCount = x.SupportCount
            })
            .OrderByDescending(c => c.CreateTime)
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return articles;
    }

    public async Task<List<ArticleGameDto>> GetArticlesByGameAsync(int pageSize, int pageIndex, int gameId)
    {
        var articles = await _evaluationContext.Articles
           .Where(art => art.GameId == gameId && art.Status == ArticleStatus.Normal)
           .Select(x => new ArticleGameDto()
           {
               ArticleId = x.ArticleId,
               AuthorId = x.UserId,
               Title = x.Title,
               SupportCount = x.SupportCount,
               Description = x.Description,
               CreateTime = x.CreateTime,
           })
           .OrderByDescending(c => c.SupportCount)
           .Skip(pageSize * (pageIndex - 1))
           .Take(pageSize)
           .AsNoTracking()
           .ToListAsync();

        return articles;
    }

    public async Task<List<ArticleTableDto>> GetUserArticlesAsync(int pageSize, int pageIndex, string userId, int? categoryId = null, bool timeDesc = true)
    {
        var queryString = _evaluationContext.Articles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new ArticleTableDto()
            {
                Id = x.ArticleId,
                CategoryId = x.CategoryTypeId,
                CreateTime = x.CreateTime,
                GameName = x.GameName,
                Title = x.Title,
                Status = x.Status
            });

        if (categoryId != null)
        {
            queryString = _evaluationContext.Articles
                .Where(x => x.CategoryTypeId == categoryId)
                .Select(x => new ArticleTableDto()
                {
                    Id = x.ArticleId,
                    CategoryId = x.CategoryTypeId,
                    CreateTime = x.CreateTime,
                    GameName = x.GameName,
                    Title = x.Title,
                    Status = x.Status
                })
                .AsNoTracking();
        }

        var result = timeDesc switch
        {
            true => await queryString.OrderByDescending(x => x.CreateTime)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(),
            false => await queryString.OrderBy(x => x.CreateTime)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(),

        };
        return result;
    }

    public async Task<EvaluationArticle> GetArticleAsync(int id)
    {
        var article = await _evaluationContext.Articles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ArticleId == id);
        return article;
    }

    public async Task<bool> IsArticleExist(int id)
    {
        var article = await _evaluationContext.Articles.AsNoTracking().FirstOrDefaultAsync(x => x.ArticleId == id);
        return article != null;
    }

    public async Task<bool> AddArticleAsync(EvaluationArticle evaluationArticle)
    {
        var random = new Random(DateTime.Now.Millisecond);
        evaluationArticle.CreateTime = DateTime.Now.ToLocalTime();

        // 随机生成一点赞和浏览量
        evaluationArticle.JoinCount = random.Next(30, 1000);
        evaluationArticle.SupportCount = random.Next(5, 100);

        await _evaluationContext.Articles.AddAsync(evaluationArticle);
        return await _evaluationContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteArticleAsync(int id)
    {
        var deleteEntity = await _evaluationContext.Articles.FindAsync(id);
        if (deleteEntity != null)
        {
            _evaluationContext.Articles.Remove(deleteEntity);
            return await _evaluationContext.SaveChangesAsync() > 0;
        }
        return false;
    }

    public async Task<bool> UpdateArticleAsync(EvaluationArticle article)
    {
        article.UpdateTime = DateTime.Now.ToLocalTime();
        _evaluationContext.Articles.Update(article);
        return await _evaluationContext.SaveChangesAsync() > 0;
    }


    /// <summary>
    ///  此方法需要用于修改实例，不建议用属性投影
    /// </summary>
    /// <param name="gameId"></param>
    /// <returns></returns>
    public async Task<List<EvaluationArticle>> GetArticlesByGameInfoAsync(int gameId)
    {
        //追踪实体修改游戏名
        var articles = await _evaluationContext.Articles
            .Where(x => x.GameId == gameId)
            .OrderByDescending(x => x.CreateTime)
            .ToListAsync();
        return articles;
    }

    /// <summary>
    ///  此方法需要用于修改实例，不建议用属性投影
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<EvaluationArticle>> GetArticlesByAuthorInfoAsync(string userId)
    {
        var articles = await _evaluationContext.Articles
            .Where(x => x.UserId == userId)
            .ToListAsync();
        return articles;
    }

    public async Task<bool> BatchUpdateArticlesAsync()
    {
        return await _evaluationContext.SaveChangesAsync() > 0;
    }

    public async Task<int> LikeArticleAsync(int articleId, string userId)
    {
        var likeRecord = await _evaluationContext.LikeRecords
            .FirstOrDefaultAsync(x => x.ArticleId == articleId && x.UserId == userId);
        //新增点赞记录
        if (likeRecord == null)
        {
            var insertEntity = new EvaluationLikeRecord() { ArticleId = articleId, UserId = userId, CreateTime = DateTime.Now.ToLocalTime() };
            _evaluationContext.LikeRecords.Add(insertEntity);
            var updateArticle = await _evaluationContext.Articles.FindAsync(articleId);
            updateArticle.SupportCount += 1;
            await _evaluationContext.SaveChangesAsync();
            return 1;
        }
        else // 已有记录
        {
            return -1;
        }
    }

    public async Task<ArticleShopDto> GetArticlesByShopItemAsync(int gameId)
    {
        return await _evaluationContext.Articles.AsNoTracking()
            .Where(x => x.GameId == gameId)
            .OrderByDescending(x => x.SupportCount)
            .Select(x => new ArticleShopDto { ArticleId = x.ArticleId, Description = x.Description, Title = x.Title, GameId = x.GameId, GameName = x.GameName })
            .FirstOrDefaultAsync();
    }

    ///// <summary>
    /////     Bulks get articles
    ///// </summary>
    ///// <param name="ids"></param>
    ///// <returns></returns>
    //private async Task<List<EvaluationArticle>> GetArticlesByIdsAsync(string ids)
    //{
    //    var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out var x), Value: x));

    //    if (!numIds.All(nid => nid.Ok)) return new List<EvaluationArticle>();

    //    var idsToSelect = numIds.Select(id => id.Value);
    //    var items = await _evaluationContext.Articles.AsNoTracking()
    //        .Where(ci => idsToSelect.Contains(ci.ArticleId))
    //        .ToListAsync();

    //    return items;
    //}

    ///// <summary>
    /////     Fill picture use default
    ///// </summary>
    ///// <param name="items"></param>
    ///// <returns></returns>
    //private List<EvaluationArticle> ChangePicsUri(List<EvaluationArticle> items)
    //{
    //    var articlePicBaseUrl = _settings.ArticlePicBaseUrl;
    //    var descriptionPicBaseUrl = _settings.DescriptionPicBaseUrl;

    //    foreach (var item in items) item.FillDefaultArticlePicture(articlePicBaseUrl, descriptionPicBaseUrl);

    //    return items;
    //}
}