namespace BackgroundWorker;

using System.Threading.Channels;

internal sealed class DefaultBackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> queue;

    public DefaultBackgroundTaskQueue(int capacity)
    {
        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
        };

        this.queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
    }

    public async ValueTask QueueAsync(Func<CancellationToken, ValueTask> workItem)
    {
        workItem = workItem ?? throw new ArgumentNullException(nameof(workItem));

        await this.queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        Func<CancellationToken, ValueTask> workItem = await this.queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }
}