namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;

public class GameRepoContextSeed
{
    public async Task SeedAsync(GameRepoContext context, ILogger<GameRepoContextSeed> logger, IWebHostEnvironment env)
    {
        var policy = CreatePolicy(logger, nameof(GameRepoContextSeed));

        await policy.ExecuteAsync(async () =>
        {
            var contentRoot = env.ContentRootPath;

            if (!context.GameCategories.Any())
            {
                await context.AddRangeAsync(GetGameCategoriesFromFile(contentRoot, logger));
                await context.SaveChangesAsync();
            }

            if (!context.GameCompanies.Any())
            {
                await context.AddRangeAsync(GetGameCompanyFromFile(contentRoot, logger));
                await context.SaveChangesAsync();
            }

            if (!context.GameTags.Any())
            {
                await context.AddRangeAsync(GetGameTagsFromFile(contentRoot, logger));
                await context.SaveChangesAsync();
            }

            if (!context.GameInfos.Any())
            {
                //初始化需要在游玩建议之前，保证约束条件达成。
                await context.AddRangeAsync(GetPreconfiguredGameInfos());
                await context.SaveChangesAsync();
            }

            if (!context.PlaySuggestions.Any())
            {
                await context.AddRangeAsync(GetPreconfiguredSuggestions());
                await context.SaveChangesAsync();
            }
        });
    }

    #region GameInfoFromFile

    private IEnumerable<GameInfo> GetGameInfosFromFile(
        string contentRootPath, GameRepoContext context, ILogger<GameRepoContextSeed> logger)
    {
        var csvFilePlaySuggestions = Path.Combine(contentRootPath, "Setup", "GameInfo.csv");

        if (!File.Exists(csvFilePlaySuggestions))
        {
            return GetPreconfiguredGameInfos();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders =
                {"name", "description", "supportplatform", "gamecompanyid","gamecategoryid","gameplaysuggestionid"};
            string[] optionalHeaders =
                { "detailspicture", "roughpicture", "averagescore", "selltime", "hotpoints" };

            csvheaders = GetHeaders(csvFilePlaySuggestions, requiredHeaders, optionalHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return GetPreconfiguredGameInfos();
        }

        var categoryTypeIdLookUp = context.GameCategories.ToDictionary(ct => ct.CategoryName, ct => ct.Id);
        var companyIdLookUp = context.GameCompanies.ToDictionary(ct => ct.CompanyName, ct => ct.Id);

        throw new NotImplementedException();
    }

    private IEnumerable<GameInfo> GetPreconfiguredGameInfos()
    {
        return new List<GameInfo>
        {
            new()
            {
               Name = "CSGO",
               Description = "《反恐精英：全球攻势》，原名Counter-Strike: Global Offensive，是一款由VALVE与Hidden Path Entertainment合作开发、Valve Software发行的第一人称射击游戏，于2012年8月21日在欧美地区正式发售，国服发布会于2017年4月11日在北京召开。  游戏为《反恐精英》系列游戏的第四款作品,游戏玩家分为反恐精英（CT阵营）与恐怖份子（T阵营）两个阵营，双方需在一个地图上进行多回合的战斗，达到地图要求目标或消灭全部敌方则取得胜利。",
               SupportPlatform = "PC/XBox",
               GameCompanyId = 14,
               GameCategoryId = 1,
               GamePlaySuggestionId = 1
            },
            new()
            {
                Name = "It Take Two",
                Description = "游玩《双人成行》，踏上生命中最疯狂的旅程。邀请好友通过远程同乐**免费游玩，体验各种搞笑而混乱的合作游戏挑战。这是一款别开生面的平台冒险游戏，只有一件事是肯定的：二人同心，其利断金。",
                SupportPlatform = "PC/XBox/PS4/PS5",
                GameCompanyId = 15,
                GameCategoryId = 15,
                GamePlaySuggestionId = 2
            }
        };
    }

    #endregion

    #region CategoriesFromFile
    private IEnumerable<GameCategory> GetGameCategoriesFromFile(string contentRootPath, ILogger<GameRepoContextSeed> logger)
    {
        string csvFileCatalogTypes = Path.Combine(contentRootPath, "Setup", "GameCategories.csv");

        if (!File.Exists(csvFileCatalogTypes))
        {
            return GetPreconfiguredCatalogTypes();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "gamecategories" };
            csvheaders = GetHeaders(csvFileCatalogTypes, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return GetPreconfiguredCatalogTypes();
        }

        return File.ReadAllLines(csvFileCatalogTypes)
                                    .Skip(1) // skip header row
                                    .SelectTry(CreateCatalogType)
                                    .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                    .Where(x => x != null);
    }

    private IEnumerable<GameCategory> GetPreconfiguredCatalogTypes()
    {
        return new List<GameCategory>
        {
            new GameCategory {CategoryName = "动作游戏"},
            new GameCategory {CategoryName = "角色扮演"}
        };
    }

    private GameCategory CreateCatalogType(string type)
    {
        type = type.Trim('"').Trim();

        if (string.IsNullOrEmpty(type))
            throw new Exception("game catalog Type Name is empty");
        return new GameCategory { CategoryName = type };
    }
    #endregion

    #region TagsFromFile
    private IEnumerable<GameTag> GetGameTagsFromFile(string contentRootPath, ILogger<GameRepoContextSeed> logger)
    {
        string csvFileCatalogTypes = Path.Combine(contentRootPath, "Setup", "GameTags.csv");

        if (!File.Exists(csvFileCatalogTypes))
        {
            return GetPreconfiguredGameTags();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "gametags" };
            csvheaders = GetHeaders(csvFileCatalogTypes, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return GetPreconfiguredGameTags();
        }

        return File.ReadAllLines(csvFileCatalogTypes)
                                    .Skip(1) // skip header row
                                    .SelectTry(CreateGameTag)
                                    .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                    .Where(x => x != null);
    }

    private IEnumerable<GameTag> GetPreconfiguredGameTags()
    {
        return new List<GameTag>
        {
            new GameTag {TagName = "FPS"},
            new GameTag {TagName = "MOBA"}
        };
    }
    private GameTag CreateGameTag(string tag)
    {
        tag = tag.Trim('"').Trim();

        if (string.IsNullOrEmpty(tag))
            throw new Exception("game tag Name is empty");
        return new GameTag { TagName = tag };
    }
    #endregion

    #region CompaniesFromFile
    private IEnumerable<GameCompany> GetGameCompanyFromFile(string contentRootPath, ILogger<GameRepoContextSeed> logger)
    {
        string csvFileCatalogTypes = Path.Combine(contentRootPath, "Setup", "GameCompanies.csv");

        if (!File.Exists(csvFileCatalogTypes))
        {
            return GetPreconfiguredGameCompanies();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "gamecompanies" };
            csvheaders = GetHeaders(csvFileCatalogTypes, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return GetPreconfiguredGameCompanies();
        }

        return File.ReadAllLines(csvFileCatalogTypes)
                                    .Skip(1) // skip header row
                                    .SelectTry(CreateGameCompany)
                                    .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                    .Where(x => x != null);
    }

    private IEnumerable<GameCompany> GetPreconfiguredGameCompanies()
    {
        return new List<GameCompany>
        {
            new GameCompany {CompanyName = "Value"},
            new GameCompany {CompanyName = "Epic"}
        };
    }

    private GameCompany CreateGameCompany(string company)
    {
        company = company.Trim('"').Trim();

        if (string.IsNullOrEmpty(company))
            throw new Exception("game company Name is empty");
        return new GameCompany { CompanyName = company };
    }

    #endregion

    #region SuggestionFromFile

    private IEnumerable<GamePlaySuggestion> GetPlaySuggestionsFromFile(
        string contentRootPath, GameRepoContext context, ILogger<GameRepoContextSeed> logger)
    {
        var csvFilePlaySuggestions = Path.Combine(contentRootPath, "Setup", "GamePlaySuggestion.csv");

        if (!File.Exists(csvFilePlaySuggestions))
        {
            return GetPreconfiguredSuggestions();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders =
                {"cpuname", "graphicscard", "disksize", "memroysize", "operationsystem", "gameid"};
            csvheaders = GetHeaders(csvFilePlaySuggestions, requiredHeaders, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return GetPreconfiguredSuggestions();
        }

        throw new NotImplementedException();
    }

    private IEnumerable<GamePlaySuggestion> GetPreconfiguredSuggestions()
    {
        return new List<GamePlaySuggestion>
        {
            new()
            {
                CPUName = "R7 5800H", GraphicsCard = "GTX 1060", DiskSize = 20, MemorySize = 16,
                OperationSystem = "Windows 11", GameId = 1
            },
            new()
            {
                CPUName = "i7 12500K", GraphicsCard = "GTX 1080", DiskSize = 50, MemorySize = 16,
                OperationSystem = "Windows 11", GameId = 2
            }
        };
    }

    #endregion

    #region MethodWith
    private string[] GetHeaders(string csvfile, string[] requiredHeaders, string[] optionalHeaders = null)
    {
        string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

        if (csvheaders.Count() < requiredHeaders.Count())
        {
            throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");
        }

        if (optionalHeaders != null)
        {
            if (csvheaders.Count() > (requiredHeaders.Count() + optionalHeaders.Count()))
            {
                throw new Exception($"csv header count '{csvheaders.Count()}'  is larger then required '{requiredHeaders.Count()}' and optional '{optionalHeaders.Count()}' headers count");
            }
        }

        foreach (var requiredHeader in requiredHeaders)
        {
            if (!csvheaders.Contains(requiredHeader))
            {
                throw new Exception($"does not contain required header '{requiredHeader}'");
            }
        }

        return csvheaders;
    }

    private AsyncRetryPolicy CreatePolicy(ILogger<GameRepoContextSeed> logger, string prefix, int retries = 3)
    {
        return Policy.Handle<MySqlConnector.MySqlException>().
            WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                }
            );
    }
    #endregion
}
