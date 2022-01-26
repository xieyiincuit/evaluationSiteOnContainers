namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure;

public class EvaluationContextSeed
{
    public async Task SeedAsync(EvaluationContext context, ILogger<EvaluationContextSeed> logger,
        IOptions<EvaluationSettings> settings, IWebHostEnvironment env)
    {
        //use policy to retry seed when connect database failure
        var policy = CreatePolicy(logger, nameof(EvaluationContextSeed));

        await policy.ExecuteAsync(async () =>
        {
            var useCustomizationData = settings.Value.UseCustomizationData;
            var contentRootPath = env.ContentRootPath;

            if (!context.Categories.Any())
            {
                await context.Categories.AddRangeAsync(useCustomizationData
                    ? GetEvaluationCategoriesFromFile(contentRootPath, logger)
                    : GetPreConfigurationEvaluationCategory());
                await context.SaveChangesAsync();
            }

            if (!context.Articles.Any())
            {
                await context.Articles.AddRangeAsync(GetPreConfigurationEvaluationArticle());
                await context.SaveChangesAsync();
            }

            if (!context.Comments.Any())
            {
                await context.Comments.AddRangeAsync(GetPreConfigurationEvaluationComment());
                await context.SaveChangesAsync();
            }
        });
    }

    private IEnumerable<EvaluationArticle> GetPreConfigurationEvaluationArticle()
    {
        return new List<EvaluationArticle>
        {
            new()
            {
                UserId = "1",
                NickName = "default user",
                Title = "《反恐精英—全球攻势》是全球最受欢迎的FPS游戏",
                Content = "枪械、地图、道具大变化！全新大行动任务玩法、全新探员、全新枪械皮肤、全新武器箱、全新印花和布章上线！登录国服购买通行证，即刻体验“激流大行动”冲浪快感！",
                CreateTime = DateTime.Now,
                Description = "TOP1 FPS GAME IN THE WORLD",
                Status = ArticleStatus.Normal,
                CategoryTypeId = 2,
                GameId = 1,
                GameName = "CSGO"
            },
            new()
            {
                UserId = "2",
                NickName = "default user",
                Title = "《双人成行》在2021年的Steam年度促销达到第一名",
                Content = "Mei and Kodi, Do some grate job to maintain their relationship",
                CreateTime = DateTime.Now.AddDays(1),
                Description = "Sell Top 1  GAME IN Steam on 2021",
                Status = ArticleStatus.Normal,
                CategoryTypeId = 2,
                GameId = 2,
                GameName = "It Take two"
            }
        };
    }

    private IEnumerable<EvaluationComment> GetPreConfigurationEvaluationComment()
    {
        return new List<EvaluationComment>
        {
            new()
            {
                Content = "我 5e 2800",
                UserId = "1",
                NickName = "Zhousl",
                CreateTime = DateTime.Now.AddHours(1),
                ArticleId = 1
            },
            new()
            {
                Content = "我 b5 2100",
                UserId = "2",
                NickName = "Hanby",
                CreateTime = DateTime.Now.AddMinutes(26),
                ArticleId = 1
            },
            new()
            {
                Content = "我觉得确实是挺有意思的",
                UserId = "1",
                NickName = "Zhousl",
                CreateTime = DateTime.Now.AddMinutes(44),
                ArticleId = 2
            },
            new()
            {
                Content = "正确的 正确的",
                UserId = "2",
                NickName = "Hanby",
                CreateTime = DateTime.Now.AddHours(1).AddMinutes(20),
                ArticleId = 2,
                IsReplay = true,
                ReplayCommentId = 3,
                ReplyUserId = "2",
                ReplyNickName = "Zhousl",
                RootCommentId = 3
            },
            new()
            {
                Content = "不是正确的 不是正确的",
                UserId = "1",
                NickName = "Zhousl",
                CreateTime = DateTime.Now.AddHours(2),
                ArticleId = 2,
                IsReplay = true,
                ReplayCommentId = 4,
                ReplyUserId = "1",
                ReplyNickName = "Hanby",
                RootCommentId = 3
            },
            new()
            {
                Content = "我觉得Zhousl说的对",
                UserId = "4",
                NickName = "Wangxb",
                CreateTime = DateTime.Now.AddHours(3),
                ArticleId = 2,
                IsReplay = true,
                ReplayCommentId = 5,
                ReplyUserId = "1",
                ReplyNickName = "Zhousl",
                RootCommentId = 3
            }
        };
    }

    private IEnumerable<EvaluationCategory> GetEvaluationCategoriesFromFile(string contentRootPath,
        ILogger<EvaluationContextSeed> logger)
    {
        var csvFileEvaluationCategories = Path.Combine(contentRootPath, "Setup", "EvaluationCategories.csv");

        if (!File.Exists(csvFileEvaluationCategories)) return GetPreConfigurationEvaluationCategory();

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "evaluationcategories" };
            csvheaders = GetHeaders(csvFileEvaluationCategories, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return GetPreConfigurationEvaluationCategory();
        }

        return File.ReadAllLines(csvFileEvaluationCategories)
            .Skip(1) // skip header row
            .SelectTry(CreateEvaluationCategory)
            .OnCaughtException(ex =>
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return null;
            })
            .Where(x => x != null);
    }

    private string[] GetHeaders(string csvfile, string[] requiredHeaders, string[] optionalHeaders = null)
    {
        var csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

        if (csvheaders.Count() < requiredHeaders.Count())
            throw new Exception(
                $"requiredHeader count '{requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");

        if (optionalHeaders != null)
            if (csvheaders.Count() > requiredHeaders.Count() + optionalHeaders.Count())
                throw new Exception(
                    $"csv header count '{csvheaders.Count()}'  is larger then required '{requiredHeaders.Count()}' " +
                    $"and optional '{optionalHeaders.Count()}' headers count");

        foreach (var requiredHeader in requiredHeaders)
            if (!csvheaders.Contains(requiredHeader))
                throw new Exception($"does not contain required header '{requiredHeader}'");

        return csvheaders;
    }

    private EvaluationCategory CreateEvaluationCategory(string category)
    {
        category = category.Trim('"').Trim();

        if (string.IsNullOrEmpty(category)) throw new Exception("evaluation category Name is empty");

        return new EvaluationCategory
        {
            CategoryType = category
        };
    }

    private IEnumerable<EvaluationCategory> GetPreConfigurationEvaluationCategory()
    {
        return new List<EvaluationCategory>
        {
            new() {CategoryType = "单机"},
            new() {CategoryType = "Xbox"},
            new() {CategoryType = "独立"},
            new() {CategoryType = "网游"},
            new() {CategoryType = "手游"}
        };
    }

    private AsyncRetryPolicy CreatePolicy(ILogger<EvaluationContextSeed> logger, string prefix, int retries = 3)
    {
        return Policy.Handle<MySqlException>().WaitAndRetryAsync(
            retries,
            retry => TimeSpan.FromSeconds(5),
            (exception, timeSpan, retry, ctx) =>
            {
                //记录重试日志
                logger.LogWarning(exception,
                    "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                    prefix, exception.GetType().Name, exception.Message, retry, retries);
            }
        );
    }
}