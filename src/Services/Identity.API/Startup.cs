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
        })
        .AddCustomIdentityStoreService(Configuration)
        .AddAspNetIdentity<ApplicationUser>();

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

        app.UseIdentityServer();
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
        builder.AddTestUsers(TestUsers.Users)
            .AddConfigurationStore(options =>
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
            });

        // not recommended for production - you need to store your key material somewhere secure
        builder.AddDeveloperSigningCredential();
        return builder;
    }
}
