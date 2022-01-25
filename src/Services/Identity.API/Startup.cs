namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API;

public class Startup
{
    public IWebHostEnvironment Environment { get; }
    public IConfiguration Configuration { get; }

    public Startup(IWebHostEnvironment environment, IConfiguration configuration)
    {
        Environment = environment;
        Configuration = configuration;
    }

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();

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
        .AddCustomIdentityStoreService(Configuration)
        .AddAspNetIdentity<ApplicationUser>()
        .AddDeveloperSigningCredential(); //开发环境使用方便

        services.AddAuthentication();

        var container = new ContainerBuilder();
        container.Populate(services);

        return new AutofacServiceProvider(container.Build());
    }

    public void Configure(IApplicationBuilder app)
    {
        if (Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
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
        });
    }
}

public static class CustomExtensionMethod
{
    /// <summary>
    /// 初始化ApplicationUserContext
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
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 15,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        return services;
    }

    /// <summary>
    /// 整合初始化Store
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IIdentityServerBuilder AddCustomIdentityStoreService(
        this IIdentityServerBuilder builder,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
        builder.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = true;
            }).Services.AddTransient<IProfileService, ProfileService<ApplicationUser>>();

        return builder;
    }
}
