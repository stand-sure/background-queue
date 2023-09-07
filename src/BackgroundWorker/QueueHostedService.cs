namespace BackgroundWorker;

internal class QueueHostedService : BackgroundService
{
    private readonly ILogger<QueueHostedService> logger;
    private readonly IBackgroundTaskQueue taskQueue;

    public QueueHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueueHostedService> logger)
    {
        this.taskQueue = taskQueue;
        this.logger = logger;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Worker stopping at {Time}", DateTimeOffset.UtcNow);
        return base.StopAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Worker running at: {Time}", DateTimeOffset.UtcNow);

        return this.ProcessTaskQueueAsync(stoppingToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Func<CancellationToken, ValueTask> workItem = await this.taskQueue.DequeueAsync(stoppingToken);

                await workItem(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error occurred executing task work item");
            }
        }
    }
}