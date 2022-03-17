using System.IdentityModel.Tokens.Jwt;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;

namespace OcelotGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            //const string authenticationProviderKey = "IdentityApiKey";

            //services.AddAuthentication()
            //    .AddJwtBearer(authenticationProviderKey, x =>
            //    {
            //        x.Authority = identityUrl;
            //        x.RequireHttpsMetadata = false;
            //        x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            //        {
            //            ValidAudiences = new[] { "ordering", "evaluation", "gamerepo", "backmanage", "identity" }
            //        };
            //    });

            services.AddOcelot()
                .AddConsul()
                .AddPolly()
                .AddConfigStoredInConsul()
                .AddCacheManager(x =>
                {
                    x.WithDictionaryHandle();
                });
            
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed(host => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseCors("CorsPolicy");
            app.UseOcelot().Wait();
        }
    }
}