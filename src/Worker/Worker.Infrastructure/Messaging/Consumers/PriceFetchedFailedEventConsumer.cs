using Common.Logging.Events;
using Common.Messaging.Events.PriceFetchEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Worker.Infrastructure.Messaging.Consumers;

public class PriceFetchedFailedEventConsumer(
    ILogger<PriceFetchedFailedEventConsumer> logger) : IConsumer<PriceFetchedFailedIntegrationEvent>
{
    public Task Consume(ConsumeContext<PriceFetchedFailedIntegrationEvent> context)
    {
        logger.LogInformation(MQEvents.PriceFetchedFailedEvent,
            "Price fetch finished for pluginId {PluginId}, @ {Date}. Reason: {Reason}", context.Message.PluginId,
            context.Message.CreatedDate, context.Message.Message);
        return Task.CompletedTask;
    }
}