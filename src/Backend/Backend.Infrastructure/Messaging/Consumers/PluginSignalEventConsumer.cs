using Backend.Application.Abstraction.Repositories;
using Backend.Domain.Entities;
using Backend.Infrastructure.Services;
using Common.Application.Queue;
using Common.Messaging.Events.PluginExecution;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Messaging.Consumers;

public class PluginSignalEventConsumer(
    IBackgroundTaskQueue taskQueue,
    ILogger<PluginSignalEventConsumer> logger) : IConsumer<PluginSignalEvent>
{
    public async Task Consume(ConsumeContext<PluginSignalEvent> context)
    {
        logger.LogInformation("PluginSignalEvent:{Signal}", context.Message.Signal.SignalType);
        await taskQueue.QueueBackgroundWorkItemAsync(context.Message);
    }
}