namespace BackgroundWorker;

using JetBrains.Annotations;

[UsedImplicitly]
internal class Program
{
    protected Program()
    {
    }

    private static void Main(string[] args)
    {
        AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromSeconds(2));

        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.ConfigureHostBuilder();

        ProgramConfiguration.ConfigureServices(builder.Services, builder.Configuration);
        IHost host = builder.Build();

        var monitorLoop = ActivatorUtilities.GetServiceOrCreateInstance<MonitorLoop>(host.Services);
        monitorLoop.StartMonitorLoop();
        
        host.Run();
    }
}