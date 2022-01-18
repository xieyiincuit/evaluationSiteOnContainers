namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Infrastructure;

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

            if (!context.GameTags.Any())
            {
                await context.AddRangeAsync(GetGameTagsFromFile(contentRoot, logger));
                await context.SaveChangesAsync();
            }

            if (!context.GameCompanies.Any())
            {
                await context.AddRangeAsync(GetGameCompanyFromFile(contentRoot, logger));
                await context.SaveChangesAsync();
            }
        });
    }

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
                                    .SelectTry(x => CreateCatalogType(x))
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
                                    .SelectTry(x => CreateGameTag(x))
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
                                    .SelectTry(x => CreateGameCompany(x))
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
