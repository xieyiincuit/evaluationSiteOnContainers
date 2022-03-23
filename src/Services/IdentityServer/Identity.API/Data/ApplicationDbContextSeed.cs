namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Data;

public class ApplicationDbContextSeed
{
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();

    public async Task SeedAsync(
        ApplicationDbContext context, IWebHostEnvironment env,
        ILogger<ApplicationDbContextSeed> logger, int? retry = 0)
    {
        try
        {
            if (!context.Users.Any())
            {
                logger.LogInformation("---- Begin Initialize User");
                await context.Users.AddRangeAsync(GetDefaultUser());
                await context.SaveChangesAsync();
                logger.LogInformation("---- Ending Initialize User");
            }

            if (!context.Roles.Any())
            {
                logger.LogInformation("---- Begin Initialize Roles");
                await context.Roles.AddRangeAsync(GetDefaultRoles());
                await context.SaveChangesAsync();
                logger.LogInformation("---- Ending Initialize Roles");
            }

            if (!context.UserRoles.Any())
            {
                logger.LogInformation("---- Begin Linking Roles and User");
                var links = await SetUserRolesAsync(context);
                await context.UserRoles.AddRangeAsync(links);
                await context.SaveChangesAsync();
                logger.LogInformation("---- Ending Linking Roles and User");
            }
        }
        catch (Exception ex)
        {
            if (retry < 10)
            {
                retry++;

                logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}",
                    nameof(ApplicationDbContext));

                await SeedAsync(context, env, logger, retry);
            }
        }
    }

    private IEnumerable<ApplicationUser> GetDefaultUser()
    {
        var admin = new ApplicationUser
        {
            Id = "A71C1391-1105-4E9A-BCBB-F70467EF070C".ToLower(),
            NickName = "胡图图",
            Avatar = "userinfopic/admin.jpg",
            Sex = Gender.Male,
            SecurityQuestion = "where are you born？",
            SecurityAnswer = "Langzhong",
            Introduction = "我的图图呢 我的图图呢",
            RegistrationDate = DateTime.Now.ToLocalTime(),
            BirthDate = new DateTime(1999, 3, 23),
            Email = "admin@outlook.com",
            PhoneNumber = "15182979603",
            UserName = "admin",
            NormalizedEmail = "ADMIN@OUTLOOK.COM",
            NormalizedUserName = "ADMIN",
            SecurityStamp = Guid.NewGuid().ToString("D")
        };
        admin.PasswordHash = _passwordHasher.HashPassword(admin, "zhou11..");

        var user = new ApplicationUser
        {
            Id = "FB9755FE-D011-435B-BD49-C4277FEB4938".ToLower(),
            NickName = "周写意",
            Avatar = "userinfopic/male.jpg",
            Sex = Gender.Male,
            SecurityQuestion = "where are you born？",
            SecurityAnswer = "Langzhong",
            Introduction = "回首昨天的自己",
            RegistrationDate = DateTime.Now.ToLocalTime(),
            BirthDate = new DateTime(1999, 7, 15),
            Email = "zhousl@outlook.com",
            PhoneNumber = "18176543788",
            UserName = "zhousl",
            NormalizedEmail = "ZHOUSL@OUTLOOK.COM",
            NormalizedUserName = "ZHOUSL",
            SecurityStamp = Guid.NewGuid().ToString("D")
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, "zhou11..");

        var evaluator = new ApplicationUser
        {
            Id = "8440A693-5DDF-4036-9972-BCA66A8002A3".ToLower(),
            NickName = "留六颗橙",
            Avatar = "userinfopic/female.jpg",
            Sex = Gender.Female,
            SecurityQuestion = "where are you born？",
            SecurityAnswer = "Langzhong",
            Introduction = "潮湿的大雾终会散去 累了就停下脚步来休息",
            RegistrationDate = DateTime.Now.ToLocalTime(),
            BirthDate = new DateTime(1999, 12, 12),
            Email = "liukc@outlook.com",
            PhoneNumber = "12346778677",
            UserName = "liukc",
            NormalizedEmail = "LIUKC@OUTLOOK.COM",
            NormalizedUserName = "LIUKC",
            SecurityStamp = Guid.NewGuid().ToString("D")
        };
        evaluator.PasswordHash = _passwordHasher.HashPassword(evaluator, "zhou11..");

        return new List<ApplicationUser>
        {
            admin,
            evaluator,
            user
        };
    }

    private IEnumerable<IdentityRole> GetDefaultRoles()
    {
        var roles = new List<IdentityRole>
        {
            new()
            {
                Id = Guid.NewGuid().ToString("N"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                Name = "administrator",
                NormalizedName = "ADMINISTRATOR"
            },
            new()
            {
                Id = Guid.NewGuid().ToString("N"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                Name = "evaluator",
                NormalizedName = "EVALUATOR"
            },
            new()
            {
                Id = Guid.NewGuid().ToString("N"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                Name = "normaluser",
                NormalizedName = "NORMALUSER"
            },
            new()
            {
                Id = Guid.NewGuid().ToString("N"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                Name = "forbiddenuser",
                NormalizedName = "FORBIDDENUSER"
            }
        };

        return roles;
    }

    private async Task<List<IdentityUserRole<string>>> SetUserRolesAsync(ApplicationDbContext context)
    {
        var admin = await context.Users.FirstOrDefaultAsync(x => x.UserName == "ADMIN".ToLower());
        var adminRole = await context.Roles.FirstOrDefaultAsync(x => x.Name == "ADMINISTRATOR".ToLower());

        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == "ZHOUSL".ToLower());
        var userRole = await context.Roles.FirstOrDefaultAsync(x => x.Name == "NORMALUSER".ToLower());

        var evaluator = await context.Users.FirstOrDefaultAsync(x => x.UserName == "LIUKC".ToLower());
        var evaluatorRole = await context.Roles.FirstOrDefaultAsync(x => x.Name == "EVALUATOR".ToLower());

        var linkRoles = new List<IdentityUserRole<string>>
        {
            new()
            {
                UserId = admin.Id,
                RoleId = adminRole.Id
            },
            new()
            {
                UserId = user.Id,
                RoleId = userRole.Id
            },
            new()
            {
                UserId = evaluator.Id,
                RoleId = evaluatorRole.Id
            }
        };
        return linkRoles;
    }
}