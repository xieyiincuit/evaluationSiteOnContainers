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
        #region ConsulRegister

        services.Configure<ServiceRegisterOptions>(Configuration.GetSection("ServiceRegister"));
        services.AddSingleton<Consul.IConsulClient>(p => new Consul.ConsulClient(cfg =>
        {
            var serviceConfiguration = p.GetRequiredService<IOptions<ServiceRegisterOptions>>().Value;
            if (serviceConfiguration.Register.HttpEndpoint != null)
            {
                cfg.Address = serviceConfiguration.Register.HttpEndpoint;
            }
        }));

        #endregion

        #region MvcSettings

        services.AddGrpc();
        services.AddControllers(options => { options.Filters.Add(typeof(HttpGlobalExceptionFilter)); })
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "evaluationSiteOnContainers - GameRepo HTTP API",
                Version = "v1",
                Description = "游戏资料服务接口文档",
                Contact = new OpenApiContact
                {
                    Name = "Zhousl",
                    Email = "zhouslthere@outlook.com"
                },
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);

            //Swagger授权
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl =
                            new Uri($"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
                        TokenUrl = new Uri($"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            {"repo-manage", "游戏信息服务管理权限"}
                        }
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
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

        services.AddHttpLogging(options =>
        {
            options.LoggingFields =
                HttpLoggingFields.RequestPath | HttpLoggingFields.RequestMethod |
                HttpLoggingFields.RequestQuery | HttpLoggingFields.RequestHeaders |
                HttpLoggingFields.RequestBody | HttpLoggingFields.ResponseStatusCode |
                HttpLoggingFields.ResponseHeaders | HttpLoggingFields.ResponseBody;
            options.RequestHeaders.Add("Authorization");

            options.RequestHeaders.Remove("Connection");
            options.RequestHeaders.Remove("User-Agent");
            options.RequestHeaders.Remove("Accept-Encoding");
            options.RequestHeaders.Remove("Accept-Language");

            options.MediaTypeOptions.AddText("application/json");
            options.RequestBodyLogLimit = 1024;
            options.ResponseBodyLogLimit = 1024;
        });

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        #endregion

        #region DbSettings

        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
            var gameRepoConnectionString = Configuration.GetConnectionString("GameRepoDbConnectString");

            services.AddDbContext<GameRepoContext>(
                dbContextOptions =>
                {
                    dbContextOptions.UseMySql(gameRepoConnectionString, serverVersion,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(
                                15,
                                TimeSpan.FromSeconds(30),
                                null);
                        });
                    //dbContextOptions.LogTo(Console.WriteLine, LogLevel.Information);
                    //dbContextOptions.EnableSensitiveDataLogging();
                    //dbContextOptions.EnableDetailedErrors();
                });

            services.AddDbContext<IntegrationEventLogContext>(
                dbContextOptions =>
                {
                    dbContextOptions.UseMySql(gameRepoConnectionString, serverVersion,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(
                                15,
                                TimeSpan.FromSeconds(30),
                                null);
                        });

                    //dbContextOptions.LogTo(Console.WriteLine, LogLevel.Information);
                    //dbContextOptions.EnableSensitiveDataLogging();
                    //dbContextOptions.EnableDetailedErrors();
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

            var factory = new ConnectionFactory
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
        services.AddScoped<IGameShopItemService, GameShopItemService>();
        services.AddScoped<IGameItemSDKService, GameItemSDKService>();
        services.AddScoped<ISDKForPlayerService, SDKForPlayerService>();
        services.AddScoped<IGameOwnerService, GameOwnerService>();

        services.AddScoped<IUnitOfWorkService, UnitOfWorkService>();

        #endregion

        #region HealthCheck

        {
            var hcBuilder = services.AddHealthChecks();

            var mqName = Configuration["EventBusSettings:UserName"];
            var mqPassword = Configuration["EventBusSettings:PassWord"];
            var mqHost = $"{Configuration["EventBusSettings:Connection"]}:{Configuration["EventBusSettings:Port"]}";

            hcBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddMySql(
                    Configuration["ConnectionStrings:GameRepoDbConnectString"],
                    "GameRepoDB-check",
                    tags: new string[] { "db", "mysql", "gamerepo" });

            hcBuilder
                .AddRedis(
                    Configuration["RedisHCCheckConnection"],
                    "redis-check",
                    tags: new string[] { "db", "redis", "gamerepo" });

            hcBuilder
                .AddRabbitMQ(
                    $"amqp://{mqName}:{mqPassword}@{mqHost}/",
                    name: "gamerepo-rabbitmqbus-check",
                    tags: new string[] { "rabbitmqbus" });

            hcBuilder.AddMinio(
                sp => sp.GetRequiredService<MinioClient>(),
                name: "gamerepo-minio",
                tags: new[] { "oss", "minio", "gamerepo" });
        }

        #endregion

        #region Authentication

        // prevent from mapping "sub" claim to nameIdentifier.
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        var identityUrl = Configuration.GetValue<string>("IdentityUrl");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = identityUrl;
            options.RequireHttpsMetadata = false;
            options.Audience = "gamerepo";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role",
                ValidIssuer = "http://identity-api",

                //用于REST通信时的受众验证失败问题
                ValidAudiences = new List<string>
                {
                    "ordering"
                }
            };
        });

        #endregion

        #region RedisSettings

        var redisConfiguration = Configuration.GetSection("Redis").Get<RedisConfiguration>();
        services.AddStackExchangeRedisExtensions<RedisNewtonsoftSerializer>(redisConfiguration);
        services.AddSingleton<IDistributedLockFactory>(provider =>
        {
            var redisClient = provider.GetRequiredService<IRedisClientFactory>();
            var redLockFactory = RedLockFactory.Create(
                new List<RedLockMultiplexer>
                    {new(redisClient.GetDefaultRedisClient().ConnectionPoolManager.GetConnection())}
            );
            return redLockFactory;
        });

        #endregion

        #region MinIO

        services.AddMinio(options =>
        {
            options.Endpoint = Configuration["Minio:Endpoint"];
            options.AccessKey = Configuration["Minio:AccessKey"];
            options.SecretKey = Configuration["Minio:SecretKey"];

        });

        #endregion

        #region HttpClients

        services.AddHttpClient<EvaluationCallService>(client =>
        {
            client.BaseAddress = new Uri(Configuration["EvaluationUrl"]);
            client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            client.Timeout = TimeSpan.FromSeconds(10);
        })
         .SetHandlerLifetime(TimeSpan.FromHours(6))
         .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(200)));

        #endregion


        var container = new ContainerBuilder();
        container.Populate(services);
        return new AutofacServiceProvider(container.Build());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
    {
        if (env.IsDevelopment()) IdentityModelEventSource.ShowPII = true;

        var pathBase = Configuration["PATH_BASE"];
        if (!string.IsNullOrEmpty(pathBase)) app.UsePathBase(pathBase);

        app.UseSwagger()
            .UseSwaggerUI(setup =>
            {
                setup.SwaggerEndpoint(
                    $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json",
                    "GameRepo.API V1");
                setup.OAuthClientId("gamereposwaggerui");
                setup.OAuthAppName("GameRepo Swagger UI");
                setup.OAuth2RedirectUrl(
                    $"http://localhost:{Configuration.GetValue<string>("SwaggerRedirectUrlPort", "50001")}/swagger/oauth2-redirect.html");
            });

        //app.UseHttpLogging();

        app.UseRouting();
        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers();
            endpoints.MapGrpcService<GameRepositoryService>();

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

        ConfigureEventBus(app);
    }

    protected virtual void ConfigureEventBus(IApplicationBuilder app)
    {
        app.ApplicationServices.GetRequiredService<IEventBus>();
    }
}