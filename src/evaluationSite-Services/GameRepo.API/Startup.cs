namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API;

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
        #region MvcSettings
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "evaluationSiteOnContainers - GameRepo HTTP API",
                Version = "v1",
                Description = "The Control of GameInfo Service HTTP API"
            });
        });
        #endregion

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });

        #region DbSettings
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
        var connectionString = Configuration.GetConnectionString("DataBaseConnectString");
        services.AddDbContext<GameRepoContext>(
            dbContextOptions => dbContextOptions
                .UseMySql(connectionString, serverVersion,
                            mySqlOptionsAction: sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                            })
                // The following three options help with debugging, but should
                // be changed or removed for production.
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
        );
        #endregion

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddScoped<IGameCategoryService, GameCategoryService>();
        services.AddScoped<IGameTagService, GameTagService>();
        services.AddScoped<IGameCompanyService, GameCompanyService>();
        services.AddScoped<IGameInfoService, GameInfoService>();
        services.AddScoped<IPlaySuggestionService, PlaySuggestionService>();

        //use autofac
        var container = new ContainerBuilder();
        container.Populate(services);
        return new AutofacServiceProvider(container.Build());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var pathBase = Configuration["PATH_BASE"];
        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);
        }

        app.UseSwagger()
           .UseSwaggerUI(setup =>
           {
               setup.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Basket.API V1");
           });

        app.UseSerilogRequestLogging();

        app.UseRouting();
        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers();
        });
    }
}
