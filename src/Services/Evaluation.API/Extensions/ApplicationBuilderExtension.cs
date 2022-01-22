namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Extensions;

public static class ApplicationBuilderExtension
{
    public static IApplicationBuilder UseBundleSwagger(this IApplicationBuilder builder, IConfiguration configuration)
    {
        var pathBase = configuration["PATH_BASE"];
        if (!string.IsNullOrEmpty(pathBase)) builder.UsePathBase(pathBase);

        builder.UseSwagger();
        builder.UseSwaggerUI(setup =>
        {
            setup.SwaggerEndpoint(
                $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json",
                "Evaluation.API V1");
            setup.RoutePrefix = string.Empty;
        });

        return builder;
    }

    public static IApplicationBuilder UseCustomEventBus(this IApplicationBuilder builder)
    {
        var eventBus = builder.ApplicationServices.GetRequiredService<IEventBus>();

        eventBus.Subscribe<GameNameChangedIntegrationEvent, GameNameChangedIntegrationEventHandler>();
        return builder;

    }
}