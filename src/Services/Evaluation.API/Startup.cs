﻿namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API;

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
            .AddCustomSwagger(Configuration)
            .AddCustomDbContext(Configuration)
            .AddCustomHealthCheck(Configuration)
            .AddCustomServicesInjection(Configuration)
            .AddCustomOptions(Configuration)
            .AddCustomMapper(Configuration)
            .AddCustomValidator(Configuration);

        //use autofac
        var container = new ContainerBuilder();
        container.Populate(services);
        return new AutofacServiceProvider(container.Build());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseBundleSwagger(Configuration);
        app.UseSerilogRequestLogging();

        app.UseRouting();
        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers();

            endpoints.MapHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self-db")
            });
        });
    }
}