namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API;

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

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .SetIsOriginAllowed(allowAllHost => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        #endregion

        #region DbSettings

        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
            var connectionString = Configuration.GetConnectionString("DataBaseConnectString");

            services.AddDbContext<GameRepoContext>(
                dbContextOptions =>
                {
                    dbContextOptions.UseMySql(connectionString, serverVersion,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 15,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);

                        });
                    dbContextOptions.LogTo(Console.WriteLine, LogLevel.Information);
                    dbContextOptions.EnableSensitiveDataLogging();
                    dbContextOptions.EnableDetailedErrors();
                });

            services.AddDbContext<IntegrationEventLogContext>(
                dbContextOptions =>
                {
                    dbContextOptions.UseMySql(connectionString, serverVersion,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 15,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        });

                    dbContextOptions.LogTo(Console.WriteLine, LogLevel.Information);
                    dbContextOptions.EnableSensitiveDataLogging();
                    dbContextOptions.EnableDetailedErrors();
                });
        }

        #endregion

        #region OptionSettings

        services.Configure<GameRepoSettings>(Configuration);
        services.Configure<EventBusSettings>(Configuration.GetSection("EventBusSettings"));
        //开发环境时可返回详细的错误信息
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to the errors property for additional details."
                };

                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json", "application/problem+xml" }
                };
            };
        });

        #endregion

        #region IntegrationSettings

        //注入IIntegrationEventLogService
        services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
            sp => (DbConnection c) => new IntegrationEventLogService(c));

        //该服务领域事件发布服务
        services.AddTransient<IGameRepoIntegrationEventService, GameRepoIntegrationEventService>();

        //注册IRabbitMQPersistentConnection服务用于设置RabbitMQ连接
        services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<EventBusSettings>>().Value;
            var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

            var factory = new ConnectionFactory()
            {

                HostName = settings.Connection,
                Port = int.Parse(settings.Port),
                ClientProvidedName = Program.AppName,
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrEmpty(settings.UserName))
                factory.UserName = settings.UserName;

            if (!string.IsNullOrEmpty(settings.PassWord))
                factory.Password = settings.PassWord;

            var retryCount = 5;
            if (!string.IsNullOrEmpty(settings.RetryCount))
                retryCount = int.Parse(settings.RetryCount);

            return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
        });

        #endregion

        #region EventBusSettings

        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var subscriptionClientName = Configuration["SubscriptionClientName"];
            var eventBusSettings = sp.GetRequiredService<IOptions<EventBusSettings>>().Value;
            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            var eventBusSubscriptionManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            var retryCount = 5;
            if (!string.IsNullOrEmpty(eventBusSettings.RetryCount))
                retryCount = int.Parse(eventBusSettings.RetryCount);

            return new EventBusRabbitMQ(
                rabbitMQPersistentConnection, logger, iLifetimeScope,
                eventBusSubscriptionManager, subscriptionClientName, retryCount);
        });

        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        #endregion

        #region GameRepoServices

        services.AddScoped<IGameCategoryService, GameCategoryService>();
        services.AddScoped<IGameTagService, GameTagService>();
        services.AddScoped<IGameCompanyService, GameCompanyService>();
        services.AddScoped<IGameInfoService, GameInfoService>();
        services.AddScoped<IPlaySuggestionService, PlaySuggestionService>();
        services.AddScoped<IUnitOfWorkService, UnitOfWorkService>();

        #endregion

        #region HealthCheck

        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddMySql(
                    Configuration["ConnectionString"],
                    name: "GameRepoDB-check",
                    tags: new string[] { "db", "mysql", "gamerepo" });

            hcBuilder
                .AddRabbitMQ(
                    $"amqp://{Configuration["EventBusSettings:Connection"]}",
                    name: "gamerepo-rabbitmqbus-check",
                    tags: new string[] { "rabbitmqbus" });
        }

        #endregion

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
               setup.SwaggerEndpoint(
                   $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json",
                   "GameRepo.API V1");
               setup.RoutePrefix = string.Empty;
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

            endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        });

        ConfigureEventBus(app);
    }

    protected virtual void ConfigureEventBus(IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        //TODO 以下添加你需要订阅的集成事件和事件处理
    }
}