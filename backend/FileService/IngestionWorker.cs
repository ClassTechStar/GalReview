/// <summary>
/// 消费 <see cref="IngestionQueue"/>：串行执行解析任务，避免无界 Task.Run。
/// 启动恢复仍由 <see cref="MongoFileStore.RecoverIncompleteJobsAsync"/> 负责。
/// </summary>
public sealed class IngestionWorker(
    IngestionQueue queue,
    IServiceProvider services,
    ILogger<IngestionWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await foreach (var jobId in queue.ReadAllAsync(stoppingToken))
            {
                if (stoppingToken.IsCancellationRequested) break;
                try
                {
                    var store = services.GetRequiredService<IFileStore>();
                    await store.ProcessJobAsync(jobId, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception error)
                {
                    logger.LogError(error, "Ingestion job {JobId} failed in background worker.", jobId);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Host shutdown.
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        queue.Complete();
        return base.StopAsync(cancellationToken);
    }
}
