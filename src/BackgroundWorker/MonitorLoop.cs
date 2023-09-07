namespace BackgroundWorker;

internal sealed class MonitorLoop
{
    private readonly IBackgroundTaskQueue taskQueue;
    private readonly ILogger<MonitorLoop> logger;
    private readonly CancellationToken cancellationToken;

    public MonitorLoop(IBackgroundTaskQueue taskQueue, ILogger<MonitorLoop> logger, IHostApplicationLifetime applicationLifetime)
    {
        this.taskQueue = taskQueue;
        this.logger = logger;
        this.cancellationToken = applicationLifetime.ApplicationStopping;
    }

    public void StartMonitorLoop()
    {
        this.logger.LogInformation($"{nameof(this.MonitorAsync)} loop is starting.");

        // Run a console user input loop in a background thread
        Task.Run(async () => await this.MonitorAsync(), this.cancellationToken);
    }

    private async ValueTask MonitorAsync()
    {
        Console.WriteLine("Hit 'w' to queue a work item");
        
        while (!this.cancellationToken.IsCancellationRequested)
        {
            ConsoleKeyInfo keyStroke = Console.ReadKey();

            if (keyStroke.Key == ConsoleKey.W)
            {
                // Enqueue a background work item
                await this.taskQueue.QueueAsync(this.BuildWorkItemAsync);
            }
        }
    }

    private async ValueTask BuildWorkItemAsync(CancellationToken token)
    {
        const int maxLoop = 3;
        var delayLoop = 0;
        var id = Guid.NewGuid().ToString("N");

        this.logger.LogInformation("Queued work item {Id} is starting", id);

        while (!token.IsCancellationRequested && delayLoop < maxLoop)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(delayLoop), token);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if the Delay is cancelled
            }

            delayLoop += 1;

            this.logger.LogInformation("Queued work item {Id} is running. {DelayLoop}/3", id, delayLoop);
        }

        string eventName = delayLoop switch
        {
            maxLoop => "complete",
            _ => "cancelled",
        };

        this.logger.LogInformation("Queued Background Task {Guid}: {Event}", id, eventName);
    }
}