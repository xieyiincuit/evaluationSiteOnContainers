namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Configurations;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("evaluation", "Evaluation API")
            {
                Scopes = { "eval-write", "eval-manage"}
            },

            new ApiResource("gamerepo", "Gamerepo API")
            {
                Scopes = { "repo-manage" }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("eval-write", "评测服务写权限",
                new List<string> {JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Id}),

            new ApiScope("eval-manage", "评测服务管理权限",
                new List<string> {JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Id}),

            new ApiScope("repo-manage", "游戏信息服务管理权限",
                new List<string> {JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Id})
        };

    public static IEnumerable<Client> Clients(Dictionary<string, string> clientsUrl) =>
        new Client[]
        {
            new Client
            {
                ClientId = "evaluationswaggerui",
                ClientName = "Evaluation Swagger UI",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RedirectUris = {$"{clientsUrl["EvaluationApi"]}/swagger/oauth2-redirect.html"},
                PostLogoutRedirectUris = {$"{clientsUrl["EvaluationApi"]}/swagger/"},
                AccessTokenLifetime = 3600 * 12,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "eval-write",
                    "eval-manage"
                }
            },
            new Client
            {
                ClientId = "gamereposwaggerui",
                ClientName = "GameRepo Swagger UI",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RedirectUris = {$"{clientsUrl["GameRepoApi"]}/swagger/oauth2-redirect.html"},
                PostLogoutRedirectUris = {$"{clientsUrl["GameRepoApi"]}/swagger/"},
                AccessTokenLifetime = 3600 * 12,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "repo-manage"
                }
            },
        };
}
