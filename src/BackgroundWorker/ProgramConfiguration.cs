namespace BackgroundWorker;

using Serilog;

internal static class ProgramConfiguration
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<QueueHostedService>();
        services.AddSingleton<MonitorLoop>();
        
        services.AddSingleton<IBackgroundTaskQueue>(_ => 
        {
            if (!int.TryParse(configuration["QueueCapacity"], out int queueCapacity))
            {
                queueCapacity = 100;
            }

            return new DefaultBackgroundTaskQueue(queueCapacity);
        });
    }

    internal static void ConfigureHostBuilder(this HostApplicationBuilder hostBuilder)
    {
        hostBuilder.UseSerilog(ConfigureHostSerilog);
    }

    private static void ConfigureHostSerilog(IConfiguration configuration, IServiceProvider provider, LoggerConfiguration loggerConfig)
    {
        loggerConfig
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(provider);
    }

    private static void UseSerilog(
        this HostApplicationBuilder builder,
        Action<IConfiguration, IServiceProvider, LoggerConfiguration> configureLogger,
        bool preserveStaticLogger = false,
        bool writeToProviders = false)
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));
        configureLogger = configureLogger ?? throw new ArgumentNullException(nameof(configureLogger));
            
        builder.Services.AddSerilog((services, loggerConfiguration) => configureLogger(builder.Configuration, services, loggerConfiguration),
                preserveStaticLogger: preserveStaticLogger,
                writeToProviders: writeToProviders);
    }
}