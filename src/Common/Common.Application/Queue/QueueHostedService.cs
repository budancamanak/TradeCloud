using Common.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.Application.Queue;

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-9.0&tabs=visual-studio
public class QueuedHostedService(
    IBackgroundTaskQueue taskQueue,
    ILogger<QueuedHostedService> logger)
    : BackgroundService
{
    public IBackgroundTaskQueue TaskQueue { get; } = taskQueue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            $"Queued Hosted Service is running.{Environment.NewLine}" +
            $"{Environment.NewLine}Tap W to add a work item to the " +
            $"background queue.{Environment.NewLine}");

        await BackgroundProcessing(stoppingToken);
    }

    protected virtual async Task ExecuteItem(IntegrationEvent workItem)
    {
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = 
                await TaskQueue.DequeueAsync(stoppingToken);

            try
            {
                await ExecuteItem(workItem);
                // await workItem(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, 
                    "Error occurred executing {WorkItem}.", nameof(workItem));
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Queued Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }
}