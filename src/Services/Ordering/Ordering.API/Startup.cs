namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

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

        #region GrpcClient

        services.AddGrpcClient<GameRepository.GameRepositoryClient>(options =>
        {
            var grpcGameRepoUrl = Configuration.GetValue("GrpcGameRepoUrl", "http://127.0.0.1:55001");
            options.Address = new Uri(grpcGameRepoUrl);
        }).AddInterceptor<GrpcExceptionInterceptor>();
        services.AddScoped<GameRepoGrpcService>();
        services.AddTransient<GrpcExceptionInterceptor>();

        #endregion

        #region MvcSettings

        services.AddControllers(options => { options.Filters.Add(typeof(HttpGlobalExceptionFilter)); })
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        //services.AddHttpLogging(options =>
        //{
        //    options.LoggingFields =
        //        HttpLoggingFields.RequestPath | HttpLoggingFields.RequestMethod |
        //        HttpLoggingFields.RequestQuery | HttpLoggingFields.RequestHeaders |
        //        HttpLoggingFields.RequestBody | HttpLoggingFields.ResponseStatusCode |
        //        HttpLoggingFields.ResponseHeaders | HttpLoggingFields.ResponseBody;
        //    options.RequestHeaders.Add("Authorization");

        //    options.RequestHeaders.Remove("Connection");
        //    options.RequestHeaders.Remove("User-Agent");
        //    options.RequestHeaders.Remove("Accept-Encoding");
        //    options.RequestHeaders.Remove("Accept-Language");

        //    options.MediaTypeOptions.AddText("application/json");
        //    options.RequestBodyLogLimit = 1024;
        //    options.ResponseBodyLogLimit = 1024;
        //});

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "evaluationSiteOnContainers - Ordering HTTP API",
                Version = "v1",
                Description = "游戏订购服务接口文档",
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
                            {"ordering-buy", "商品下单服务权限"},
                            {"ordering-manage", "订单管理权限"}
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
            options.Audience = "ordering";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role",
                ValidIssuer = "http://identity-api"
            };
        });

        #endregion

        #region RedisOptions

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

        #region HealthChecks

        var hcBuilder = services.AddHealthChecks();

        hcBuilder
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddRedis(
                Configuration["RedisHCCheckConnection"],
                "redis-check",
                tags: new string[] { "db", "redis", "ordering" });

        #endregion

        #region HttpClients

        services.AddHttpClient<GameRepoHttpClient>(client =>
            {
                client.BaseAddress = new Uri(Configuration["GameRepoUrl"]);
                client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                client.Timeout = TimeSpan.FromSeconds(5);
            })
            .SetHandlerLifetime(TimeSpan.FromHours(6))
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(5, _ => TimeSpan.FromMilliseconds(200)));

        services.AddHttpContextAccessor();

        #endregion

        #region Events
        services.AddTransient<IOrderIntegrationEventService, OrderIntegrationEventService>();

        services.Configure<OrderingSettings>(Configuration);
        services.Configure<EventBusSettings>(Configuration.GetSection("EventBusSettings"));

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

        //连接RabbitMq集成事件总线服务
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

        var container = new ContainerBuilder();
        container.Populate(services);
        return new AutofacServiceProvider(container.Build());
    }

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
                    "Ordering.API V1");
                setup.OAuthClientId("orderingswaggerui");
                setup.OAuthAppName("Ordering Swagger UI");
                setup.OAuth2RedirectUrl(
                    $"http://{Configuration.GetValue<string>("SwaggerRedirectUrl", "localhost")}:{Configuration.GetValue<string>("SwaggerRedirectUrlPort", "50002")}/swagger/oauth2-redirect.html");
            });

        //Debug时使用
        //app.UseHttpLogging();

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