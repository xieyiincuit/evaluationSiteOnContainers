using ILogger = Serilog.ILogger;

var configuration = GetConfiguration();

Log.Logger = CreateSerilogLogger(configuration);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", AppName);
    var host = BuildWebHost(configuration, args);

    Log.Information("Applying migrations ({ApplicationContext})...", AppName);
    host.MigrateMySqlDbContext<EvaluationContext>((context, services) =>
    {
        var env = services.GetService<IWebHostEnvironment>();
        var settings = services.GetService<IOptions<EvaluationSettings>>();
        var logger = services.GetRequiredService<ILogger<EvaluationContextSeed>>();
        new EvaluationContextSeed().SeedAsync(context, logger, settings, env).Wait();
    });
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
        .ConfigureKestrel(options =>
        {
            var ports = GetDefinedPorts(configuration);
            options.Listen(IPAddress.Any, ports.httpPort,
                listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; });

            options.Listen(IPAddress.Any, ports.grpcPort,
                listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
        })
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
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}")
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}


IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddEnvironmentVariables();

    var config = builder.Build();
    return builder.Build();
}

(int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
{
    var grpcPort = config.GetValue("GRPC_PORT", 55000);
    var port = config.GetValue("PORT", 50000);
    return (port, grpcPort);
}

public partial class Program
{
    public static string Namespace = typeof(Startup).Namespace;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}