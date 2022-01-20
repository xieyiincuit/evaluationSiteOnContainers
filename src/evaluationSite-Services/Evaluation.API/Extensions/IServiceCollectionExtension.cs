namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Extensions;

public static class IServiceCollectionExtension
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
        });

        return services;
    }

    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EvaluationContext>(options =>
        {
            var connectionString = configuration["ConnectionStrings:DataBaseConnectString"];
            options.UseSqlServer(connectionString,
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                });
        });
        return services;
    }

    public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options => { options.Filters.Add(typeof(HttpGlobalExceptionFilter)); })
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

        return services;
    }

    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddSqlServer(
                configuration["ConnectionStrings:DataBaseConnectString"],
                "SELECT 1;",
                "sql",
                HealthStatus.Degraded,
                new[] {"db", "sql", "sqlserver"});
        return services;
    }

    public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EvaluationSettings>(configuration);
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
                    ContentTypes = {"application/problem+json", "application/problem+xml"}
                };
            };
        });

        return services;
    }

    public static IServiceCollection AddCustomServicesInjection(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IEvaluationArticle, EvaluationArticleService>();
        services.AddScoped<IEvaluationCategory, EvaluationCategoryService>();
        services.AddScoped<IEvaluationComment, EvaluationCommentService>();
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
}