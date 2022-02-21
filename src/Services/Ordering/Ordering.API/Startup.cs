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
        #region GrpcClient
      
        services.AddGrpc();
        services.AddGrpcClient<GameRepository.GameRepositoryClient>(options =>
        {
            var grpcGameRepoUrl = Configuration.GetValue("GrpcGameRepoUrl", "http://127.0.0.1:55001");
            options.Address = new Uri(grpcGameRepoUrl);
        }).AddInterceptor<GrpcExceptionInterceptor>();
        services.AddScoped(typeof(GrpcRepoCallService));
        services.AddScoped(typeof(GrpcBaseCallService));
        services.AddTransient<GrpcExceptionInterceptor>();

        #endregion

        #region MvcSettings

        services.AddControllers(options =>
           {
               options.Filters.Add(typeof(HttpGlobalExceptionFilter));
           })
           .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

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

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "evaluationSiteOnContainers - Ordering HTTP API",
                Version = "v1",
                Description = "The Control of Ordering Service HTTP API"
            });

            //Swagger授权
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl =
                            new Uri($"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
                        TokenUrl = new Uri($"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),
                        Scopes = new Dictionary<string, string>()
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
            options.TokenValidationParameters = new TokenValidationParameters()
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
                new List<RedLockMultiplexer> { new RedLockMultiplexer(redisClient.GetDefaultRedisClient().ConnectionPoolManager.GetConnection()) }
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
                name: "redis-check",
                tags: new string[] { "db", "redis", "ordering" });

        #endregion

        #region HttpClients

        services.AddHttpClient<RepoCallService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["GameRepoUrl"]);
                client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(500);
            })
            .SetHandlerLifetime(TimeSpan.FromHours(6))
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(5, _ => TimeSpan.FromMilliseconds(200)));

        services.AddHttpContextAccessor();

        #endregion

        var container = new ContainerBuilder();
        container.Populate(services);
        return new AutofacServiceProvider(container.Build());
    }

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
                    "Ordering.API V1");
                setup.OAuthClientId("orderingswaggerui");
                setup.OAuthAppName("Ordering Swagger UI");
                setup.OAuth2RedirectUrl("http://localhost:50002/swagger/oauth2-redirect.html");
            });

        app.UseHttpLogging();

        app.UseRouting();
        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers();
            endpoints.MapGrpcService<OrderingService>();

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
    }
}