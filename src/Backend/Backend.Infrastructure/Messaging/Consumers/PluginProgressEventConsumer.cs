using Backend.Application.Abstraction.Repositories;
using Backend.Infrastructure.Services;
using Common.Application.Queue;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Messaging.Events.PluginExecution;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Messaging.Consumers;

public class PluginProgressEventConsumer : IConsumer<PluginProgressEvent>
{
    private readonly RedisProgressPublisher _redisPublisher;
    private readonly ILogger<PluginProgressEventConsumer> _logger;

    public PluginProgressEventConsumer(RedisProgressPublisher redisPublisher, ILogger<PluginProgressEventConsumer> logger)
    {
        _redisPublisher = redisPublisher;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PluginProgressEvent> context)
    {
        var msg = context.Message;
        
        // Write to Redis (fast, non-blocking)
        await _redisPublisher.PublishAsync(msg.PluginId, msg.Progress);
        
        _logger.LogDebug("Progress {Progress}% for plugin {Id} written to Redis", msg.Progress, msg.PluginId);
    }
}
