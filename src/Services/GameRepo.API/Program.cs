var configuration = GetConfiguration();

Log.Logger = CreateSerilogLogger(configuration);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", Program.AppName);
    var host = BuildWebHost(configuration, args);

    Log.Information("Applying migrations ({ApplicationContext})...", Program.AppName);
    host.MigrateDbContext<GameRepoContext>((context, services) =>
    {
        var env = services.GetService<IWebHostEnvironment>();
        var logger = services.GetRequiredService<ILogger<GameRepoContextSeed>>();
        new GameRepoContextSeed().SeedAsync(context, logger, env).Wait();
    });
    host.MigrateDbContext<IntegrationEventLogContext>((_, _) => { });
    Log.Information("Migrations Applied ({ApplicationContext})...", Program.AppName);

    Log.Information("Starting web host ({ApplicationContext})...", Program.AppName);
    host.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", Program.AppName);
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
Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .Enrich.WithProperty("ApplicationContext", Program.AppName)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    return builder.Build();
}

public partial class Program
{
    public static string Namespace = typeof(Startup).Namespace;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}
