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
        var admin = new ApplicationUser()
        {
            NickName = "Simple",
            Avatar = "default",
            Sex = Gender.Male,
            SecurityQuestion = "where are you born？",
            SecurityAnswer = "Langzhong",
            Introduction = "I am admin",
            RegistrationDate = DateTime.Now.ToLocalTime(),
            BirthOfYear = 2000,
            BirthOfMonth = 7,
            BirthOfDay = 15,
            Email = "zhouslthere@outlook.com",
            Id = Guid.NewGuid().ToString(),
            PhoneNumber = "15182979603",
            UserName = "zhouslthere",
            NormalizedEmail = "ZHOUSLTHERE@OUTLOOK.COM",
            NormalizedUserName = "ZHOUSLTHERE",
            SecurityStamp = Guid.NewGuid().ToString("D")
        };
        admin.PasswordHash = _passwordHasher.HashPassword(admin, "zhou11..");

        var user = new ApplicationUser()
        {
            NickName = "Zywoo",
            Avatar = "default",
            Sex = Gender.Male,
            SecurityQuestion = "where are you born？",
            SecurityAnswer = "Guangdong",
            Introduction = "I am user",
            RegistrationDate = DateTime.Now.ToLocalTime(),
            BirthOfYear = 2000,
            BirthOfMonth = 4,
            BirthOfDay = 28,
            Email = "zywoothere@outlook.com",
            Id = Guid.NewGuid().ToString(),
            PhoneNumber = "18176543788",
            UserName = "zywoothere",
            NormalizedEmail = "ZYWOOTHERE@OUTLOOK.COM",
            NormalizedUserName = "ZYWOOTHERE",
            SecurityStamp = Guid.NewGuid().ToString("D")
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, "zhou11..");

        var evaluator = new ApplicationUser()
        {
            NickName = "Niko",
            Avatar = "default",
            Sex = Gender.Male,
            SecurityQuestion = "where are you born？",
            SecurityAnswer = "Chengdu",
            Introduction = "I am evaluator",
            RegistrationDate = DateTime.Now.ToLocalTime(),
            BirthOfYear = 1998,
            BirthOfMonth = 7,
            BirthOfDay = 15,
            Email = "nikothere@outlook.com",
            Id = Guid.NewGuid().ToString(),
            PhoneNumber = "12346778677",
            UserName = "nikothere",
            NormalizedEmail = "NIKOTHERE@OUTLOOK.COM",
            NormalizedUserName = "NIKOTHERE",
            SecurityStamp = Guid.NewGuid().ToString("D")
        };
        evaluator.PasswordHash = _passwordHasher.HashPassword(evaluator, "zhou11..");

        return new List<ApplicationUser>()
            {
                admin,
                evaluator,
                user
            };
    }

    private IEnumerable<IdentityRole> GetDefaultRoles()
    {
        var roles = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                    Name = "administrator",
                    NormalizedName = "ADMINISTRATOR"
                },
                new IdentityRole()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                    Name = "evaluator",
                    NormalizedName = "EVALUATOR"
                },
                new IdentityRole()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                    Name = "normaluser",
                    NormalizedName = "NORMALUSER"
                },
                new IdentityRole()
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
        var admin = await context.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == "ZHOUSLTHERE".ToUpper());
        var adminRole = await context.Roles.FirstOrDefaultAsync(x => x.NormalizedName == "ADMINISTRATOR".ToUpper());

        var user = await context.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == "ZYWOOTHERE".ToUpper());
        var userRole = await context.Roles.FirstOrDefaultAsync(x => x.NormalizedName == "NORMALUSER".ToUpper());

        var evaluator = await context.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == "NIKOTHERE".ToUpper());
        var evaluatorRole = await context.Roles.FirstOrDefaultAsync(x => x.NormalizedName == "EVALUATOR".ToUpper());

        var linkRoles = new List<IdentityUserRole<string>>()
            {
                new IdentityUserRole<string>()
                {
                    UserId = admin.Id,
                    RoleId = adminRole.Id
                },
                new IdentityUserRole<string>()
                {
                    UserId = user.Id,
                    RoleId = userRole.Id
                },
                new IdentityUserRole<string>()
                {
                    UserId = evaluator.Id,
                    RoleId = evaluatorRole.Id
                },
            };
        return linkRoles;
    }
}
