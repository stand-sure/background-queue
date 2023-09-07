namespace BackgroundWorker;

using Serilog;

internal static class ProgramConfiguration
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddHostedService<Worker>();
    }   
    
    internal static void ConfigureHostBuilder(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog(ConfigureHostSerilog);
    }

    private static void ConfigureHostSerilog(HostBuilderContext context, IServiceProvider provider, LoggerConfiguration loggerConfig)
    {
        loggerConfig
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(provider);
    }
}