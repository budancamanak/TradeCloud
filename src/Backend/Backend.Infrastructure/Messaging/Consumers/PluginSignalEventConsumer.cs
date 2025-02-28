using Backend.Application.Abstraction.Repositories;
using Backend.Domain.Entities;
using Common.Messaging.Events.PluginExecution;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Messaging.Consumers;

public class PluginSignalEventConsumer(
    IPluginOutputRepository pluginOutputRepository,
    IPluginExecutionRepository pluginExecutionRepository,
    ILogger<PluginSignalEventConsumer> logger) : IConsumer<PluginSignalEvent>
{
    public async Task Consume(ConsumeContext<PluginSignalEvent> context)
    {
        logger.LogInformation("Consuming PluginSignalEvent> Saving signal for {}: {}", context.Message.PluginId,
            context.Message.Signal);
        var plugin = await pluginExecutionRepository.GetByIdAsync(context.Message.PluginId);
        if (plugin == null) return;
        var mr = await pluginOutputRepository.AddAsync(new PluginOutput
        {
            PluginId = context.Message.PluginId,
            PluginSignal = context.Message.Signal.SignalType,
            CreatedDate = context.Message.CreatedDate,
            SignalDate = context.Message.Signal.SignalDate
        });
        logger.LogInformation("Consumed PluginSignalEvent> Saving signal result for {}: {} - {}",
            context.Message.PluginId,
            context.Message.Signal, mr);
    }
}