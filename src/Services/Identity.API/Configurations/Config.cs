namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Configurations;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiResource> ApiScopes =>
        new ApiResource[]
        {
            new ApiResource("evaluation", "评测服务相关API"),
            new ApiResource("gamerepo", "游戏信息服务相关API"),
        };

    public static IEnumerable<Client> Clients(Dictionary<string, string> clientsUrl) =>
        new Client[]
        {
            new Client
            {
                ClientId = "gamereposwaggerui",
                ClientName = "GameRepo Swagger UI",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RedirectUris = {$"{clientsUrl["GameRepoApi"]}/swagger/oauth2-redirect.html"},
                PostLogoutRedirectUris = {$"{clientsUrl["GameRepoApi"]}/swagger/"},

                AllowedScopes =
                {
                    "gamerepo"
                }
            },

            new Client
            {
                ClientId = "evaluationswaggerui",
                ClientName = "Evaluation Swagger UI",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RedirectUris = {$"{clientsUrl["EvaluationApi"]}/swagger/oauth2-redirect.html"},
                PostLogoutRedirectUris = {$"{clientsUrl["EvaluationApi"]}/swagger/"},

                AllowedScopes =
                {
                    "evaluation"
                }
            },
        };
}
