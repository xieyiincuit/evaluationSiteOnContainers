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
            logger.LogWarning("file path:{path} can't find csv file, PlaySuggestions initialize may wrong", csvFilePlaySuggestions);
            return GetPreconfiguredGameInfos();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders =
                {"name", "description", "supportplatform", "gamecompanyid", "gamecategoryid", "gameplaysuggestionid"};
            string[] optionalHeaders =
                {"detailspicture", "roughpicture", "averagescore", "selltime", "hotpoints"};

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
                Name = "永劫无间",
                Description = "《永劫无间》是由24 Entertainment制作发行的一款动作游戏新作。在域外之地聚窟州，神秘的力量等待着来自各大文明的武者。玩家需要运用丰富的武器战胜敌人，而世界的真相将向最终的胜者展开。",
                SupportPlatform = "PC/PS5",
                GameCompanyId = 16,
                GameCategoryId = 3,
                GamePlaySuggestionId = 1,
                AverageScore = 7.3,
                HotPoints = 2467,
                DetailsPicture = "gameinfopic/gamerepo/gameyongjiebig.jpg",
                RoughPicture = "gameinfopic/gamerepo/gameyongjie.jpg",
            },
            new()
            {
                Name = "艾尔登法环",
                Description = "《艾尔登法环》是一款以正统黑暗奇幻世界为舞台的动作RPG游戏。走进辽阔的场景与地下迷宫探索未知，挑战困难重重的险境，享受克服困境时的成就感吧。不仅如此，登场角色之间的利害关系谱成的群像剧，更是不容错过。",
                SupportPlatform = "PC/XBox/PS4/PS5",
                GameCompanyId = 17,
                GameCategoryId = 3,
                GamePlaySuggestionId = 2,
                AverageScore = 9.1,
                HotPoints = 12387,
                DetailsPicture = "gameinfopic/gamerepo/gamelaotouhuanbig.jpg",
                RoughPicture = "gameinfopic/gamerepo/gamelaotouhuan.jpg"
            },
            new()
            {
                Name = "战地5",
                Description = "《战地5》是一款由EA制作并发行的第一人称射击游戏，为《战地》的正统续作。本作的多人模式增加了多个如同电影大片般的角色动画，玩家叱诧战场，还得提防背后敌人的袭击，震撼的画质加上角色动画，堪比电影大片。",
                SupportPlatform = "PC/XBox/PS4",
                GameCompanyId = 15,
                GameCategoryId = 1,
                GamePlaySuggestionId = 3,
                AverageScore = 8.1,
                HotPoints = 5543,
                DetailsPicture = "gameinfopic/gamerepo/gamezhandiwubig.jpg",
                RoughPicture = "gameinfopic/gamerepo/gamezhandiwu.jpg"
            },
            new()
            {
                Name = "Apex英雄",
                Description = "《Apex英雄》（Apex Legends）是一款免费的战术竞技游戏，适用经典的战术竞技游戏规则，由泰坦陨落制作组Respawn研发。在游戏中，我们将领略到拥有强大技能的传说级角色，并与他们携手，在这充满危险与机遇的土地上，为荣誉、为利益、为名望，携手而战吧！",
                SupportPlatform = "PC/Switch/PS4/Xbox",
                GameCompanyId = 15,
                GameCategoryId = 1,
                GamePlaySuggestionId = 4,
                AverageScore = 8.6,
                HotPoints = 8977,
                DetailsPicture = "gameinfopic/gamerepo/gameapexbig.jpg",
                RoughPicture = "gameinfopic/gamerepo/gameapex.jpg"
            },
            new()
            {
                Name = "幻塔",
                Description = "《幻塔》是完美世界游戏旗下Hotta Studio自主研发并打造的轻科幻开放世界手游。融合去标签化角色塑造，影视级动作捕捉，高自由度世界探索玩法、轻科幻美术风格、多样场景互动解谜元素与自由职业战斗，讲述有关拯救与毁灭的末世故事。",
                SupportPlatform = "IOS/Android",
                GameCompanyId = 4,
                GameCategoryId = 4,
                AverageScore = 6.3,
                HotPoints = 2567,
                DetailsPicture = "gameinfopic/gamerepo/gamehuantabig.jpg",
                RoughPicture = "gameinfopic/gamerepo/gamehuanta.jpg"
            },
            new()
            {
                Name = "王者荣耀",
                Description = "《王者荣耀》是一款由腾讯天美工作室开发，腾讯游戏运营的MOBA手游。在游戏中赵云、昭君、曹操、嬴政，5000年东方英雄集结战场，大战一触即发。浓郁的东方元素、创新性3V3设定、1条路对决、10分钟快意决胜负，再配以两指即可玩转的简易操作，本作MOBA类手游注入全新活力。",
                SupportPlatform = "IOS/Android/Switch",
                GameCompanyId = 1,
                GameCategoryId = 4,
                AverageScore = 7.3,
                HotPoints = 29322,
                DetailsPicture = "gameinfopic/gamerepo/gamewangzhebig.jpg",
                RoughPicture = "gameinfopic/gamerepo/gamewangzhe.jpg"
            },
            new()
            {
                Name = "永进",
                Description = "《永进》是一款3D解密冒险游戏，故事讲述了少女雅的故事。她独自生活在一座孤岛上，突然有一日被拉入了充满着诡异气息的异空间之中，各种复杂困难的谜题接踵而至，她不得不迎面而上。随着她的前进，她的记忆渐渐复苏，她为何独自一人？她到底身在何方？一切关于这个世界的关于自身的谜题终将被慢慢解开。",
                SupportPlatform = "PC/Switch/PS4/PS5/Xbox",
                GameCompanyId = 18,
                GameCategoryId = 8,
                GamePlaySuggestionId = 5,
                AverageScore = 7.6,
                HotPoints = 1287,
                DetailsPicture = "gameinfopic/gamerepo/gameyongjinbig.jpg",
                RoughPicture = "gameinfopic/gamerepo/gameyongjin.jpg"
            },
            new()
            {
                Name = "GT7",
                Description = "《GT7》是由Polyphony Digital工作室制作的一款赛车竞速游戏，该作为《GT6》的正统续作。本作除了延续《GT》系列“最真实赛车游戏”的美誉外，游戏还将逼真运行所有画面，《GT7》会坚持更高的原创性，使游戏更具新作魅力。",
                SupportPlatform = "PS4/PS5",
                GameCompanyId = 12,
                GameCategoryId = 5,
                AverageScore = 8.8,
                HotPoints = 8964,
                DetailsPicture = "gameinfopic/gamerepo/gamegt7big.jpg",
                RoughPicture = "gameinfopic/gamerepo/gamegt7.jpg"
            },
            new()
            {
                Name = "最终幻想：起源",
                Description = "《最终幻想：起源》由野村哲也、野岛一成与Team Ninja协力开发，将让玩家们从一个大胆的新角度来看《最终幻想》。本作中玩家将跟随主角Jack和他的伙伴Ash、Jed，在打开混沌神殿的大门之时，打败混沌的决心在三人的胸中燃烧。然而他们也有着一丝疑虑——他们真的是预言中的光之战士吗。",
                SupportPlatform = "PC/PS4/PS5/Xbox",
                GameCompanyId = 19,
                GameCategoryId = 4,
                GamePlaySuggestionId = 9,
                AverageScore = 8.5,
                HotPoints = 6794,
                DetailsPicture = "gameinfopic/gamerepo/gamezzhxbig.jpg",
                RoughPicture = "gameinfopic/gamerepo/gamezzhx.jpg"
            },
        };
    }

    #endregion

    #region CategoriesFromFile

    private IEnumerable<GameCategory> GetGameCategoriesFromFile(string contentRootPath,
        ILogger<GameRepoContextSeed> logger)
    {
        var csvFileCatalogTypes = Path.Combine(contentRootPath, "Setup", "GameCategories.csv");

        if (!File.Exists(csvFileCatalogTypes))
        {
            logger.LogWarning("file path:{path} can't find csv file, CatalogTypes initialize may wrong", csvFileCatalogTypes);
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
            .OnCaughtException(ex =>
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return null;
            })
            .Where(x => x != null);
    }

    private IEnumerable<GameCategory> GetPreconfiguredCatalogTypes()
    {
        return new List<GameCategory>
        {
            new() {CategoryName = "动作游戏"},
            new() {CategoryName = "角色扮演"}
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
        var csvFileGameTags = Path.Combine(contentRootPath, "Setup", "GameTags.csv");

        if (!File.Exists(csvFileGameTags))
        {
            logger.LogWarning("file path:{path} can't find csv file, GameTags initialize may wrong", csvFileGameTags);
            return GetPreconfiguredGameTags();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "gametags" };
            csvheaders = GetHeaders(csvFileGameTags, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return GetPreconfiguredGameTags();
        }

        return File.ReadAllLines(csvFileGameTags)
            .Skip(1) // skip header row
            .SelectTry(CreateGameTag)
            .OnCaughtException(ex =>
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return null;
            })
            .Where(x => x != null);
    }

    private IEnumerable<GameTag> GetPreconfiguredGameTags()
    {
        return new List<GameTag>
        {
            new() {TagName = "FPS"},
            new() {TagName = "MOBA"}
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
        var csvFileCatalogTypes = Path.Combine(contentRootPath, "Setup", "GameCompanies.csv");

        if (!File.Exists(csvFileCatalogTypes))
        {
            logger.LogWarning("file path:{path} can't find csv file, CatalogTypes initialize may wrong", csvFileCatalogTypes);
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
            .OnCaughtException(ex =>
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return null;
            })
            .Where(x => x != null);
    }

    private IEnumerable<GameCompany> GetPreconfiguredGameCompanies()
    {
        return new List<GameCompany>
        {
            new() {CompanyName = "Value"},
            new() {CompanyName = "Epic"}
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
            logger.LogWarning("file path:{path} can't find csv file, PlaySuggestions initialize may wrong", csvFilePlaySuggestions);
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
                CPUName = "Intel i5-8500", GraphicsCard = "GeForce GTX 1060",
                DiskSize = 50, MemorySize = 12,
                OperationSystem = "Windows 10", GameId = 1
            },
            new()
            {
                CPUName = "Intel i5-8400", GraphicsCard = "GeForce GTX 1060",
                DiskSize = 60, MemorySize = 12,
                OperationSystem = "Windows 10", GameId = 2
            },
            new()
            {
                CPUName = "Intel i5-6600k", GraphicsCard = "GeForce GTX 1050",
                DiskSize = 50, MemorySize = 8,
                OperationSystem = "Windows 10", GameId = 3
            },
            new()
            {
                CPUName = "Intel i5-3570K", GraphicsCard = "GeForce GTX 970",
                DiskSize = 22, MemorySize = 8,
                OperationSystem = "Windows 10", GameId = 4
            },
            new()
            {
                CPUName = "Intel i5-2500K", GraphicsCard = "GeForce GTX 970",
                DiskSize = 4, MemorySize = 8,
                OperationSystem = "Windows 10", GameId = 7
            },
            new()
            {
                CPUName = "Intel i7-8700", GraphicsCard = "GeForce GTX 1660",
                DiskSize = 80, MemorySize = 16,
                OperationSystem = "Windows 10", GameId = 9
            }
        };
    }

    #endregion

    #region MethodWith

    private string[] GetHeaders(string csvfile, string[] requiredHeaders, string[] optionalHeaders = null)
    {
        var csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

        if (csvheaders.Count() < requiredHeaders.Count())
            throw new Exception(
                $"requiredHeader count '{requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");

        if (optionalHeaders != null)
            if (csvheaders.Count() > requiredHeaders.Count() + optionalHeaders.Count())
                throw new Exception(
                    $"csv header count '{csvheaders.Count()}'  is larger then required '{requiredHeaders.Count()}' and optional '{optionalHeaders.Count()}' headers count");

        foreach (var requiredHeader in requiredHeaders)
            if (!csvheaders.Contains(requiredHeader))
                throw new Exception($"does not contain required header '{requiredHeader}'");

        return csvheaders;
    }

    private AsyncRetryPolicy CreatePolicy(ILogger<GameRepoContextSeed> logger, string prefix, int retries = 3)
    {
        return Policy.Handle<MySqlException>().WaitAndRetryAsync(
            retries,
            retry => TimeSpan.FromSeconds(5),
            (exception, timeSpan, retry, ctx) =>
            {
                logger.LogWarning(exception,
                    "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                    prefix, exception.GetType().Name, exception.Message, retry, retries);
            }
        );
    }

    #endregion
}