using Backend.Infrastructure.Services;
using Common.Messaging.Events.PluginExecution;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Messaging.Consumers;

public class PluginSignalEventConsumer(
    RedisPluginSignalPublisher _redisPublisher,
    ILogger<PluginSignalEventConsumer> logger) : IConsumer<PluginSignalEvent>
{
    public async Task Consume(ConsumeContext<PluginSignalEvent> context)
    {
        var msg = context.Message;

        // Write to Redis (fast, non-blocking)
        await _redisPublisher.PublishAsync(msg.PluginId, msg);

        // logger.LogInformation("Signal {signal}% for plugin {Id} written to Redis", msg.Signal, msg.PluginId);
    }
}