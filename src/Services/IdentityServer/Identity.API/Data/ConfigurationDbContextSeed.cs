using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;

namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Data;
public class ConfigurationDbContextSeed
{
    public async Task SeedAsync(ConfigurationDbContext context, IConfiguration configuration)
    {
        //callbacks urls from config:
        var clientUrls = new Dictionary<string, string>
        {
            {"EvaluationApi", configuration.GetValue<string>("EvaluationApiClient")},
            {"GameRepoApi", configuration.GetValue<string>("GameRepoApiClient")},
            {"OrderingApi", configuration.GetValue<string>("OrderingApiClient")},
            {"BackManageApi", configuration.GetValue<string>("BackManageApiClient")},
        };

        if (!context.Clients.Any())
        {
            foreach (var client in Config.Clients(clientUrls))
            {
                context.Clients.Add(client.ToEntity());
            }
            await context.SaveChangesAsync();
        }
        // Checking always for old redirects to fix existing deployments
        // to use new swagger-ui redirect uri as of v3.0.0
        else
        {
            List<ClientRedirectUri> oldRedirects = (await context.Clients.Include(c => c.RedirectUris).ToListAsync())
                .SelectMany(c => c.RedirectUris)
                .Where(ru => ru.RedirectUri.EndsWith("/o2c.html"))
                .ToList();

            if (oldRedirects.Any())
            {
                foreach (var ru in oldRedirects)
                {
                    ru.RedirectUri = ru.RedirectUri.Replace("/o2c.html", "/oauth2-redirect.html");
                    context.Update(ru.Client);
                }
                await context.SaveChangesAsync();
            }
        }

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.IdentityResources)
            {
                context.IdentityResources.Add(resource.ToEntity());
            }
            await context.SaveChangesAsync();
        }

        if (!context.ApiScopes.Any())
        {
            foreach (var api in Config.ApiScopes)
            {
                context.ApiScopes.Add(api.ToEntity());
            }

            await context.SaveChangesAsync();
        }

        if (!context.ApiResources.Any())
        {
            foreach (var api in Config.ApiResources)
            {
                context.ApiResources.Add(api.ToEntity());
            }

            await context.SaveChangesAsync();
        }
    }
}

