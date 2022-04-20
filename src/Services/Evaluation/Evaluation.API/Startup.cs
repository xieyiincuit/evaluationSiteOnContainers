using Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Grpc;

namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public virtual IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services
            .AddCustomMvc(Configuration)
            .AddCustomGrpcClient(Configuration)
            .AddCustomRegister(Configuration)
            .AddCustomSwagger(Configuration)
            .AddCustomDbContext(Configuration)
            .AddCustomHealthCheck(Configuration)
            .AddCustomServicesInjection(Configuration)
            .AddCustomAuth(Configuration)
            .AddCustomOptions(Configuration)
            .AddCustomIntegrationEvent(Configuration)
            .AddCustomHttpClient(Configuration)
            .AddCustomEventBus(Configuration)
            .AddCustomMapper(Configuration)
            .AddCustomValidator(Configuration)
            .AddCustomRedis(Configuration)
            .AddCustomMinio(Configuration);

        //use autofac
        var container = new ContainerBuilder();
        container.Populate(services);
        return new AutofacServiceProvider(container.Build());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
    {
        if (env.IsDevelopment()) IdentityModelEventSource.ShowPII = true;

        app.UseBundleSwagger(Configuration);

        //Debug Using
        //app.UseHttpLogging();

        app.UseRouting();
        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers();
            endpoints.MapGrpcService<EvaluationService>();

            endpoints.MapHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        });

        var consul = app.ApplicationServices.GetRequiredService<Consul.IConsulClient>();
        var serviceConfiguration = app.ApplicationServices.GetRequiredService<IOptions<ServiceRegisterOptions>>();
        app.RegisterService(serviceConfiguration, consul, lifetime);

        app.UseCustomEventBus();
    }
}