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
                Scopes = {"eval-write", "eval-manage"}
            },

            new ApiResource("gamerepo", "Gamerepo API")
            {
                Scopes = {"repo-manage"}
            },

            new ApiResource("ordering", "Ordering API")
            {
                Scopes = {"ordering-buy", "ordering-manage"}
            },

            new ApiResource("backmanage", "BackManage API")
            {
                Scopes = {"back-manage"}
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
                new List<string> {JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Id}),

            new ApiScope("ordering-buy", "商品下单服务权限",
                new List<string> {JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Id}),

            new ApiScope("ordering-manage", "订单管理权限",
                new List<string> {JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Id}),

            new ApiScope("back-manage", "网站后台管理权限",
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
                AccessTokenLifetime = 7200,
                AlwaysSendClientClaims = true,
                AlwaysIncludeUserClaimsInIdToken = true,
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
                AccessTokenLifetime = 7200,
                AlwaysSendClientClaims = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "repo-manage"
                }
            },
            new Client
            {
                ClientId = "orderingswaggerui",
                ClientName = "Ordering Swagger UI",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RedirectUris = {$"{clientsUrl["OrderingApi"]}/swagger/oauth2-redirect.html"},
                PostLogoutRedirectUris = {$"{clientsUrl["OrderingApi"]}/swagger/"},
                AccessTokenLifetime = 7200,
                AlwaysSendClientClaims = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "ordering-buy",
                    "ordering-manage",
                }
            },
            new Client
            {
                ClientId = "backmanageswaggerui",
                ClientName = "BackManage Swagger UI",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RedirectUris = {$"{clientsUrl["BackManageApi"]}/swagger/oauth2-redirect.html"},
                PostLogoutRedirectUris = {$"{clientsUrl["BackManageApi"]}/swagger/"},
                AccessTokenLifetime = 7200,
                AlwaysSendClientClaims = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "back-manage",
                }
            }
        };
}
