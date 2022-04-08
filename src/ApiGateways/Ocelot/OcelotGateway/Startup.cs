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
            services.AddOcelot()
                .AddConsul() // 注册中心
                .AddPolly()  // 重试机制
                .AddConfigStoredInConsul()
                .AddCacheManager(x =>
                {
                    // 缓存
                    x.WithDictionaryHandle();
                });

            // 网关跨域
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