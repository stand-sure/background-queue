namespace BackgroundWorker;

internal interface IBackgroundTaskQueue
{
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
    ValueTask QueueAsync(Func<CancellationToken, ValueTask> workItem);
}