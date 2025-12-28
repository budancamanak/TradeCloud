using Backend.Application.Abstraction.Repositories;
using Backend.Infrastructure.Services;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Messaging.Events.PluginExecution;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Messaging.Consumers;

public class AnalysisExecutionProgressEventConsumer(
    IAnalysisExecutionRepository repository,
    ICacheService cache,
    IProgressNotifier _notifier,
    ILogger<AnalysisExecutionProgressEventConsumer> logger) : IConsumer<AnalysisExecutionProgressEvent>
{
    private static object lockObject = new object();

    public async Task Consume(ConsumeContext<AnalysisExecutionProgressEvent> context)
    {
        // cachedProgress += context.Message.Increment;
        // await cache.SetAsync(CacheKeyGenerator.PluginProgressEventConsumer(context.Message.AnalysisExecutionId),
        //     cachedProgress, TimeSpan.FromMinutes(15));
        lock (lockObject)
        {
            cache.Increment(CacheKeyGenerator.AnalysisProgressEventConsumer(context.Message.AnalysisExecutionId),
                context.Message.Increment);
        }

        var cachedProgress =
            await cache.GetAsync<double>(
                CacheKeyGenerator.AnalysisProgressEventConsumer(context.Message.AnalysisExecutionId));

        logger.LogInformation(
            "Consuming AnalysisExecutionProgressEvent > Setting Analysis[{AnalysisId}] progress to {CachedProgress} ",
            context.Message.AnalysisExecutionId, cachedProgress);
        await repository.SetAnalysisExecutionProgress(context.Message.AnalysisExecutionId, context.Message.Increment,
            0);
        await _notifier.NotifyProgressAsync(context.Message.AnalysisExecutionId, cachedProgress);
        // if (cachedProgress >= 100)
        // {
        //     await cache.RemoveAsync(CacheKeyGenerator.AnalysisProgressEventConsumer(context.Message.AnalysisExecutionId));
        // }


        // logger.LogInformation("Consumed PluginProgressEvent > Setting plugin[{}] progress to {} : {}",
        //     context.Message.PluginId, context.Message.Progress, mr);
    }
}