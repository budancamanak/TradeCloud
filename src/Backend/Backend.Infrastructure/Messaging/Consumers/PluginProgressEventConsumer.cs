using Backend.Application.Abstraction.Repositories;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Messaging.Events.PluginExecution;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Messaging.Consumers;

public class PluginProgressEventConsumer(
    IPluginExecutionRepository repository,
    ICacheService cache,
    ILogger<PluginProgressEventConsumer> logger) : IConsumer<PluginProgressEvent>
{
    public async Task Consume(ConsumeContext<PluginProgressEvent> context)
    {
        // logger.LogInformation("Consuming PluginProgressEvent > Setting plugin[{}] progress to {} ",
        //     context.Message.PluginId, context.Message.Progress);
        await cache.SetAsync(CacheKeyGenerator.PluginProgressEventConsumer(context.Message.PluginId),
            context.Message.Progress, TimeSpan.FromMinutes(15));
        var cachedProgress =
            await cache.GetAsync<double>(CacheKeyGenerator.PluginProgressEventConsumer(context.Message.PluginId));
        if (cachedProgress % 10 == 0)
        {
            var mr = await repository.SetPluginProgress(context.Message.PluginId, context.Message.Progress);
        }

        // logger.LogInformation("Consumed PluginProgressEvent > Setting plugin[{}] progress to {} : {}",
        //     context.Message.PluginId, context.Message.Progress, mr);
    }
}