namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

        var subjectId = subject.Claims.FirstOrDefault(x => x.Type == "sub").Value;

        var user = await _userManager.FindByIdAsync(subjectId);
        if (user == null)
            throw new ArgumentException("Invalid subject identifier");

        var claims = GetClaimsFromUser(user);
        context.IssuedClaims = claims.Result.ToList();
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

        var subjectId = subject.Claims.FirstOrDefault(x => x.Type == "sub").Value;
        var user = await _userManager.FindByIdAsync(subjectId);

        context.IsActive = false;

        if (user != null)
        {
            if (_userManager.SupportsUserSecurityStamp)
            {
                var securityStamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value)
                    .SingleOrDefault();
                if (securityStamp != null)
                {
                    var dbSecurityStamp = await _userManager.GetSecurityStampAsync(user);
                    if (dbSecurityStamp != securityStamp)
                        return;
                }
            }

            context.IsActive =
                !user.LockoutEnabled ||
                !user.LockoutEnd.HasValue ||
                user.LockoutEnd <= DateTime.Now;
        }
    }

    //额外设置 可以发送到Client
    //AlwaysSendClientClaims = true,
    //AlwaysIncludeUserClaimsInIdToken = true
    private async Task<IEnumerable<Claim>> GetClaimsFromUser(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtClaimTypes.Subject, user.Id),
            new(JwtClaimTypes.Name, user.UserName),
            new(JwtClaimTypes.Role, roles.FirstOrDefault() ?? string.Empty)
        };

        if (!string.IsNullOrWhiteSpace(user.NickName))
            claims.Add(new Claim(JwtClaimTypes.NickName, user.NickName));

        if (!string.IsNullOrWhiteSpace(user.Avatar))
            claims.Add(new Claim("avatar", user.Avatar));

        if (_userManager.SupportsUserEmail)
            claims.AddRange(new[]
            {
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
            });

        if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            claims.AddRange(new[]
            {
                new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false",
                    ClaimValueTypes.Boolean)
            });

        return claims;
    }
}