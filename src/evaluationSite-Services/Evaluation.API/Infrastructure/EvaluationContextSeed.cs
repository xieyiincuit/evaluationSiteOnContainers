namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure;

public class EvaluationContextSeed
{
    public async Task SeedAsync(EvaluationContext context, ILogger<EvaluationContextSeed> logger)
    {
        //use policy to retry seed when connect database failure
        var policy = CreatePolicy(logger, nameof(EvaluationContextSeed));

        await policy.ExecuteAsync(async () =>
        {
            if (!context.Categories.Any())
            {
                await context.Categories.AddRangeAsync(GetPreconfigurationEvaluationCategory());
                await context.SaveChangesAsync();
            }

            if (!context.Articles.Any())
            {
                await context.Articles.AddRangeAsync(GetPreconfigurationEvaluationArticle());
                await context.SaveChangesAsync();
            }

            if (!context.Comments.Any())
            {
                await context.Comments.AddRangeAsync(GetPreconfigurationEvaluationComment());
                await context.SaveChangesAsync();
            }
        });
    }

    private IEnumerable<EvaluationArticle> GetPreconfigurationEvaluationArticle()
    {
        return new List<EvaluationArticle>()
        {
            new EvaluationArticle()
            {
                Author = "Zhousl",
                Title = "《反恐精英—全球攻势》在全球的FPS领域中影响都是非常大的",
                Content = "枪械、地图、道具大变化！全新大行动任务玩法、全新探员、全新枪械皮肤、全新武器箱、全新印花和布章上线！登录国服购买通行证，即刻体验“激流大行动”冲浪快感！",
                CreateTime = DateTime.Now,
                Description = "TOP1 FPS GAME IN THE WORLD",
                Status = ArticleStatus.Normal,
                CategoryTypeId = 4,
                GameId = 1,
                GameName = "CSGO",
            },
            new EvaluationArticle()
            {
                Author = "Hanby",
                Title = "《双人成行》在2021年的Steam年度促销达到第一名",
                Content = "Mei and Kodi, Do some grate job to maintain thire relationship",
                CreateTime = DateTime.Now.AddDays(1),
                Description = "Sell Top 1  GAME IN Steam on 2021",
                Status = ArticleStatus.Normal,
                CategoryTypeId = 4,
                GameId = 2,
                GameName = "It Take two",
            }
        };
    }

    private IEnumerable<EvaluationComment> GetPreconfigurationEvaluationComment()
    {
        return new List<EvaluationComment>()
        {
            new EvaluationComment()
            {
                Content = "5e 2800",
                UserId = 1,
                NickName = "Luocl",
                CreateTime = DateTime.Now.AddHours(1),
                ArticleId = 1
            },
            new EvaluationComment()
            {
                Content = "b5 2100",
                UserId = 2,
                NickName = "Chenxy",
                CreateTime = DateTime.Now.AddMinutes(26),
                ArticleId = 1
            },
            new EvaluationComment()
            {
                Content = "我觉得确实是挺有意思的",
                UserId = 3,
                NickName = "Zhousl",
                CreateTime = DateTime.Now.AddMinutes(44),
                ArticleId = 2
            },
            new EvaluationComment()
            {
                Content = "正确的 正确的",
                UserId = 4,
                NickName = "Hanbaoyi",
                CreateTime = DateTime.Now.AddHours(1).AddMinutes(20),
                ArticleId = 2,
                IsReplay = true,
                ReplayCommentId = 3,
                ReplyUserId = 3,
                ReplyNickName = "Zhousl",
                RootCommentId = 3
            },
            new EvaluationComment()
            {
                Content = "不是正确的 不是正确的",
                UserId = 3,
                NickName = "Zhousl",
                CreateTime = DateTime.Now.AddHours(2),
                ArticleId = 2,
                IsReplay = true,
                ReplayCommentId = 4,
                ReplyUserId = 4,
                ReplyNickName = "Hanbaoyi",
                RootCommentId = 3
            },
            new EvaluationComment()
            {
                Content = "我觉得Zhousl说的对",
                UserId = 5,
                NickName = "Wangxb",
                CreateTime = DateTime.Now.AddHours(3),
                ArticleId = 2,
                IsReplay = true,
                ReplayCommentId = 5,
                ReplyUserId = 3,
                ReplyNickName = "Zhousl",
                RootCommentId = 3
            },
        };
    }

    private IEnumerable<EvaluationCategory> GetPreconfigurationEvaluationCategory()
    {
        return new List<EvaluationCategory>()
        {
            new EvaluationCategory() { CategoryType = "单机" },
            new EvaluationCategory() { CategoryType = "Xbox" },
            new EvaluationCategory() { CategoryType = "独立游戏" },
            new EvaluationCategory() { CategoryType = "网游" },
            new EvaluationCategory() { CategoryType = "手游" }
        };
    }

    private AsyncRetryPolicy CreatePolicy(ILogger<EvaluationContextSeed> logger, string prefix, int retries = 3)
    {
        return Policy.Handle<SqlException>().
            WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    //记录重试日志
                    logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                        prefix, exception.GetType().Name, exception.Message, retry, retries);
                }
            );
    }
}

