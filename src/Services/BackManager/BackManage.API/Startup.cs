namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

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
                Title = "evaluationSiteOnContainers - BackManage HTTP API",
                Version = "v1",
                Description = "后台管理服务接口文档",
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
                            {"back-manage", "网站后台管理权限"}
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

        #region Authentication

        // prevent from mapping "sub" claim to nameIdentifier.
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        //在Docker容器内 需配置Docker DNS
        //这里与回调地址IdentityUrlExternal区分
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

            options.TokenValidationParameters = new TokenValidationParameters
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
            .AddMySql(
                Configuration["BackDbConnectString"],
                "mysql-check",
                tags: new string[] { "db", "mysql", "backmanage" });

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

        services.AddHttpClient<IdentityHttpClient>(client =>
            {
                client.BaseAddress = new Uri(Configuration["IdentityUrl"]);
                client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .SetHandlerLifetime(TimeSpan.FromHours(6))
            .AddPolicyHandler(retryPolicy);

        services.AddHttpContextAccessor();

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
                    "BackManage.API V1");
                setup.OAuthClientId("backmanageswaggerui");
                setup.OAuthAppName("BackManage Swagger UI");
                setup.OAuth2RedirectUrl(
                    $"http://{Configuration.GetValue<string>("SwaggerRedirectUrl", "localhost")}:{Configuration.GetValue<string>("SwaggerRedirectUrlPort", "50004")}/swagger/oauth2-redirect.html");
            });

        //需要详细信息时可以使用
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
    }
}