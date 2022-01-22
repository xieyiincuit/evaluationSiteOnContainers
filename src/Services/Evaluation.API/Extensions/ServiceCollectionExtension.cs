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
        });

        return services;
    }

    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
        var connectionString = configuration.GetConnectionString("DataBaseConnectString");
        services.AddDbContext<EvaluationContext>(
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
            .AddMySql(
                configuration["ConnectionStrings:DataBaseConnectString"],
                "mysql",
                HealthStatus.Degraded,
                new[] {"db", "sql", "mysql"});
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
        services.AddScoped<IEvaluationArticleService, EvaluationArticleService>();
        services.AddScoped<IEvaluationCategoryService, EvaluationCategoryService>();
        services.AddScoped<IEvaluationCommentService, EvaluationCommentService>();
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