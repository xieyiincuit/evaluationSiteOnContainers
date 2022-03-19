namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API;

public class Startup
{
    public Startup(IWebHostEnvironment environment, IConfiguration configuration)
    {
        Environment = environment;
        Configuration = configuration;
    }

    public IWebHostEnvironment Environment { get; }
    public IConfiguration Configuration { get; }

    public IServiceProvider ConfigureServices(IServiceCollection services)
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

        services.AddControllers(options => { options.Filters.Add(typeof(HttpGlobalExceptionFilter)); })
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);
        services.AddControllersWithViews();

        services.AddSwaggerGen(options =>
       {
           options.SwaggerDoc("v1", new OpenApiInfo
           {
               Title = "evaluationSiteOnContainers - Identity HTTP API",
               Version = "v1",
               Description = "The Identity Service HTTP API"
           });
       });

        #endregion

        #region IdentityServer

        services.AddCustomIdentityDbContext(Configuration);
        services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;

                options.IssuerUri = "http://identity-api";
                options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
            })
            .AddAspNetIdentity<ApplicationUser>() //这个DI注入在前面需要，后面的IProfile才会复写成功
            .AddCustomIdentityStoreService(Configuration)
            .AddDeveloperSigningCredential(); //开发环境使用证书

        services.Configure<IdentityOptions>(options =>
        {
            // 用户在添加时，密码的要求。
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // 锁定账户
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        });

        #endregion

        #region SelfJwtAuth

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        var identityUrl = Configuration.GetValue<string>("IdentityUrl");
        services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role",
                    ValidIssuer = "http://identity-api",
                    ValidAudiences = new List<string>
                    {
                        "ordering",
                        "evaluation",
                        "backmanage",
                        "gamerepo"
                    }
                };
            });

        #endregion

        #region ExternalProvider

        services.AddAuthentication()
            .AddGoogle("Google", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                options.ClientId = "<insert here>";
                options.ClientSecret = "<insert here>";
            }).AddMicrosoftAccount("Microsoft", microsoftOptions =>
            {
                microsoftOptions.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                microsoftOptions.ClientId = "<insert here>";
                microsoftOptions.ClientSecret = "<insert here>";
            }).AddQQ("QQ", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                options.ClientId = "<insert here>";
                options.ClientSecret = "<insert here>";
            }).AddWeixin("Weixin", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                options.ClientId = "<insert here>";
                options.ClientSecret = "<insert here>";
            });

        #endregion

        #region HealthChecks

        {
            var mqName = Configuration["EventBusSettings:UserName"];
            var mqPassword = Configuration["EventBusSettings:PassWord"];
            var mqHost = $"{Configuration["EventBusSettings:Connection"]}:{Configuration["EventBusSettings:Port"]}";

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddSqlServer(Configuration.GetConnectionString("IdentityConnection"),
                    name: "IdentityDB-check",
                    tags: new string[] { "IdentityDB" })
                .AddRabbitMQ(
                    $"amqp://{mqName}:{mqPassword}@{mqHost}/",
                    name: "identity-rabbitmqbus-check",
                    tags: new string[] { "rabbitmqbus" });
        }

        #endregion

        #region Events

        services.AddDbContext<IntegrationEventLogContext>(
            dbContextOptions =>
            {
                dbContextOptions.UseSqlServer(Configuration.GetConnectionString("IdentityConnection"),
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

        //注入IIntegrationEventLogService
        services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
            sp => (DbConnection c) => new IntegrationEventLogService(c));

        //该服务领域事件发布服务
        services.AddTransient<IIdentityIntegrationEventService, IdentityIntegrationEventService>();

        services.Configure<IdentitySettings>(Configuration);
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

        #region MinIO

        services.AddMinio(options =>
        {
            options.Endpoint = Configuration["Minio:Endpoint"];
            options.AccessKey = Configuration["Minio:AccessKey"];
            options.SecretKey = Configuration["Minio:SecretKey"];

        });

        #endregion

        var container = new ContainerBuilder();
        container.Populate(services);
        return new AutofacServiceProvider(container.Build());
    }

    public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
    {
        if (Environment.IsDevelopment()) app.UseDeveloperExceptionPage();
        var pathBase = Configuration["PATH_BASE"];
        if (!string.IsNullOrEmpty(pathBase)) app.UsePathBase(pathBase);

        app.UseSwagger()
          .UseSwaggerUI(setup =>
          {
              setup.SwaggerEndpoint(
                  $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json",
                  "Identity.API V1");
              setup.RoutePrefix = "swagger";
          });

        app.UseStaticFiles();

        // Make work identity server redirections in Edge and latest versions of browser.
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
            await next();
        });

        app.UseRouting();
        app.UseForwardedHeaders();
        app.UseIdentityServer();

        // Fix a problem with chrome. Chrome enabled a new feature "Cookies without SameSite must be secure", 
        // the cookies should be expired from https, but in this app, the internal communication in aks and docker compose is http.
        // To avoid this problem, the policy of cookies should be in Lax mode.
        app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

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
        //获取连接接口并订阅channel
        app.ApplicationServices.GetRequiredService<IEventBus>();
    }
}

public static class CustomExtensionMethod
{
    /// <summary>
    ///     初始化ApplicationUserContext
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomIdentityDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add framework services.
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(
                        15,
                        TimeSpan.FromSeconds(30),
                        null);
                }));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        return services;
    }

    /// <summary>
    ///     整合初始化Store
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IIdentityServerBuilder AddCustomIdentityStoreService(
        this IIdentityServerBuilder builder,
        IConfiguration configuration)
    {
        var identityConnectionString = configuration.GetConnectionString("IdentityConnection");
        var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
        builder.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(identityConnectionString,
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        sqlOptions.EnableRetryOnFailure(
                            15,
                            TimeSpan.FromSeconds(30),
                            null);
                    });
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(identityConnectionString,
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        sqlOptions.EnableRetryOnFailure(
                            15,
                            TimeSpan.FromSeconds(30),
                            null);
                    });
                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = true;
            }).Services.AddTransient<IProfileService, ProfileService>();

        return builder;
    }
}