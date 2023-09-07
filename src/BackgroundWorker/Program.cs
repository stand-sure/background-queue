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

        IHostBuilder builder = Host.CreateDefaultBuilder(args);
        
        builder.ConfigureHostBuilder();

        builder.ConfigureServices(ProgramConfiguration.ConfigureServices);
        IHost host = builder.Build();

        host.Run();
    }
}