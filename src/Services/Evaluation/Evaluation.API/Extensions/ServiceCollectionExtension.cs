namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "evaluationSiteOnContainers - Evaluation HTTP API",
                Version = "v1",
                Description = "The Evaluation Service HTTP API"
            });

            //Swagger授权
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl =
                            new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
                        TokenUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            {"eval-write", "评测服务写权限"},
                            {"eval-manage", "评测服务管理权限"}
                        }
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });

        return services;
    }

    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
        var connectionString = configuration.GetConnectionString("EvaluationDbConnectString");
        services.AddDbContext<EvaluationContext>(
            dbContextOptions => dbContextOptions
                .UseMySql(connectionString, serverVersion,
                    sqlOptions => { sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null); })
                // The following three options help with debugging, but should
                // be changed or removed for production.
                //.LogTo(Console.WriteLine, LogLevel.Information)
                //.EnableSensitiveDataLogging()
                //.EnableDetailedErrors()
        );
        return services;
    }

    public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                options.Filters.Add(typeof(ValidateModelStateFilter));
            })
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true)
            .AddFluentValidation(options =>
            {
                //禁用框架的validator
                options.DisableDataAnnotationsValidation = true;
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

        return services;
    }

    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services,
        IConfiguration configuration)
    {
        var mqName = configuration["EventBusSettings:UserName"];
        var mqPassword = configuration["EventBusSettings:PassWord"];
        var mqHost = $"{configuration["EventBusSettings:Connection"]}:{configuration["EventBusSettings:Port"]}";

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddMySql(
                configuration["ConnectionStrings:EvaluationDbConnectString"],
                "EvaluationDB-check",
                HealthStatus.Degraded,
                new[] { "db", "evaluation", "mysql" })
            .AddRabbitMQ(
                $"amqp://{mqName}:{mqPassword}@{mqHost}/",
                name: "evaluation-rabbitmqbus-check",
                tags: new[] { "rabbitmqbus" });

        return services;
    }

    public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EvaluationSettings>(configuration);
        services.Configure<EventBusSettings>(configuration.GetSection("EventBusSettings"));
        services.AddOptions();
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

        return services;
    }

    public static IServiceCollection AddCustomServicesInjection(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IEvaluationArticleService, EvaluationArticleService>();
        services.AddScoped<IEvaluationCategoryService, EvaluationCategoryService>();
        services.AddScoped<IEvaluationCommentService, EvaluationCommentService>();
        return services;
    }

    public static IServiceCollection AddCustomHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IdentityCallService>(client =>
            {
                client.BaseAddress = new Uri(configuration["IdentityUrl"]);
                client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                client.Timeout = TimeSpan.FromSeconds(2);
            })
            .SetHandlerLifetime(TimeSpan.FromHours(6))
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(200)));

        services.AddHttpContextAccessor();
        return services;
    }

    public static IServiceCollection AddCustomMapper(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }

    public static IServiceCollection AddCustomValidator(this IServiceCollection services, IConfiguration configuration)
    {
        //在确定不存在循环引用的情况下，使用单例模式可以提升每次request的服务加载时间      
        services.AddValidatorsFromAssemblyContaining<ArticleAddDto>(ServiceLifetime.Singleton);
        return services;
    }

    public static IServiceCollection AddCustomIntegrationEvent(this IServiceCollection services,
        IConfiguration configuration)
    {
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
        return services;
    }

    public static IServiceCollection AddCustomEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];
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

        //订阅实现接口
        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        //Handler服务注入，这里切记要注入，不然AutoFac映射不到对应的Handler
        services.AddTransient<GameNameChangedIntegrationEventHandler>();
        services.AddTransient<NickNameChangedIntegrationEventHandler>();
        return services;
    }

    public static IServiceCollection AddCustomAuth(this IServiceCollection services, IConfiguration configuration)
    {
        // prevent from mapping "sub" claim to nameIdentifier.
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        var identityUrl = configuration.GetValue<string>("IdentityUrl");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = identityUrl;
            options.RequireHttpsMetadata = false;
            options.Audience = "evaluation";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role",

                ValidateIssuer = true,
                ValidIssuer = "http://identity-api"
            };
        });

        return services;
    }

    public static IServiceCollection AddCustomRegister(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ServiceRegisterOptions>(configuration.GetSection("ServiceRegister"));
        services.AddSingleton<Consul.IConsulClient>(p => new Consul.ConsulClient(cfg =>
        {
            var serviceConfiguration = p.GetRequiredService<IOptions<ServiceRegisterOptions>>().Value;
            if (serviceConfiguration.Register.HttpEndpoint != null)
            {
                cfg.Address = serviceConfiguration.Register.HttpEndpoint;
            }
        }));
        return services;
    }
}