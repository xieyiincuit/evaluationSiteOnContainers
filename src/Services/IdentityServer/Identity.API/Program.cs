using ILogger = Serilog.ILogger;

var configuration = GetConfiguration();

Log.Logger = CreateSerilogLogger(configuration);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", AppName);
    var host = BuildWebHost(configuration, args);

    Log.Information("Applying migrations ({ApplicationContext})...", AppName);
    host.MigrateSqlServerDbContext<ApplicationDbContext>((context, services) =>
    {
        var env = services.GetService<IWebHostEnvironment>();
        var logger = services.GetRequiredService<ILogger<ApplicationDbContextSeed>>();
        new ApplicationDbContextSeed().SeedAsync(context, env, logger).Wait();
    });
    host.MigrateSqlServerDbContext<PersistedGrantDbContext>((_, _) => { });
    host.MigrateSqlServerDbContext<ConfigurationDbContext>((context, services) =>
    {
        var logger = services.GetRequiredService<ILogger<ConfigurationDbContextSeed>>();
        new ConfigurationDbContextSeed().SeedAsync(context, configuration, logger).Wait();
    });
    host.MigrateSqlServerDbContext<IntegrationEventLogContext>((_, _) => { });
    Log.Information("Migrations Applied ({ApplicationContext})...", AppName);

    Log.Information("Starting web host ({ApplicationContext})...", AppName);
    host.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

//Create Web Host
IWebHost BuildWebHost(IConfiguration configuration, string[] args)
{
    return WebHost.CreateDefaultBuilder(args)
        .CaptureStartupErrors(false)
        .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
        .UseStartup<Startup>()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseSerilog()
        .Build();
}

//Set Logging Middleware
ILogger CreateSerilogLogger(IConfiguration configuration)
{
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
        .Enrich.WithProperty("ApplicationContext", AppName)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}",
            theme: AnsiConsoleTheme.Code)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddEnvironmentVariables();

    return builder.Build();
}

public partial class Program
{
    public static string Namespace = typeof(Startup).Namespace;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}