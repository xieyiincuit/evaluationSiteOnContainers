namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Configurations;

public static class Config
{
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource("role", "角色", new List<string> {JwtClaimTypes.Role}),
            new IdentityResource("nickname", "昵称", new List<string> {JwtClaimTypes.NickName}),
        };
    }


    public static IEnumerable<ApiResource> GetApiResources()
    {
        return new List<ApiResource>
        {
            new("evaluation", "Evaluation API")
            {
                Scopes = {"eval-write", "eval-manage"}
            },
            new("gamerepo", "Gamerepo API")
            {
                Scopes = {"repo-manage"}
            },
            new("ordering", "Ordering API")
            {
                Scopes = {"ordering-buy", "ordering-manage"}
            },
            new("backmanage", "BackManage API")
            {
                Scopes = {"back-manage"}
            }
        };
    }

    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new("eval-write", "评测服务写权限",
                new List<string> {JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Id}),
            new("eval-manage", "评测服务管理权限",
                new List<string> {JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Id}),
            new("repo-manage", "游戏信息服务管理权限",
                new List<string> {JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Id}),
            new("ordering-buy", "商品下单服务权限",
                new List<string> {JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Id}),
            new("ordering-manage", "订单管理权限",
                new List<string> {JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Id}),
            new("back-manage", "网站后台管理权限",
                new List<string> {JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Id}),

        };
    }


    public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
    {
        return new List<Client>
        {
            #region SwaggerClient

            new()
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
            new()
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
            new()
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
                    "ordering-manage"
                }
            },
            new()
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
                    "back-manage"
                }
            },

            #endregion

            #region WebSPAClient

            // JavaScript Client Web
            new Client
            {
                ClientId = "evaluationsitevuejs",
                ClientName = "Evaluation SPA OpenId Client",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RequireConsent = false,
               
                RedirectUris = {$"{clientsUrl["WebSPA"]}/callback"},
                PostLogoutRedirectUris = {$"{clientsUrl["WebSPA"]}/"},
                AllowedCorsOrigins = {$"{clientsUrl["WebSPA"]}"},

                AccessTokenLifetime = 3600 * 12,
                UpdateAccessTokenClaimsOnRefresh = true,
                AllowOfflineAccess = true,
                
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "eval-write",
                    "ordering-buy"
                }
            },

            #endregion
        };
    }
}