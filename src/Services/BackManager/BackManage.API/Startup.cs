namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public virtual IServiceProvider ConfigureServices(IServiceCollection services)
    {
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
                Title = "evaluationSiteOnContainers - BackManage HTTP API",
                Version = "v1",
                Description = "The Control of BackManage Service HTTP API"
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
                            {"backend-manage", "网站后台管理权限"}
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

        #region DbSettings

        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
            var gameRepoConnectionString = Configuration["BackDbConnectString"];

            services.AddDbContext<BackManageContext>(
                dbContextOptions =>
                {
                    dbContextOptions.UseMySql(gameRepoConnectionString, serverVersion,
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
            options.Audience = "backmanage";
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                NameClaimType = "name",
                RoleClaimType = "role",
                ValidIssuer = "http://identity-api"
            };
        });

        #endregion

        #region HealthChecks

        var hcBuilder = services.AddHealthChecks();

        hcBuilder
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddRedis(
                Configuration["RedisConnectionString"],
                name: "redis-check",
                tags: new string[] { "db", "redis", "backmanage" });

        #endregion

        #region ServicesInjection

        services.AddScoped<IApprovalService, ApproveService>();
        services.AddScoped<IBannedService, BannedService>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        #endregion

        #region HttpClients

        var retryPolicy = Policy.Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(response =>
                response.StatusCode == HttpStatusCode.InternalServerError ||
                response.StatusCode == HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(300));

        services.AddHttpClient<IdentityClientService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["IdentityUrl"]);
                client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(500);
            })
            .SetHandlerLifetime(TimeSpan.FromHours(6))
            .AddPolicyHandler(retryPolicy);

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
                    "BackManage.API V1");
                setup.OAuthClientId("backmanageswaggerui");
                setup.OAuthAppName("BackManage Swagger UI");
                setup.OAuth2RedirectUrl("http://localhost:50004/swagger/oauth2-redirect.html");
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