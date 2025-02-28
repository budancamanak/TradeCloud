using Common.Messaging.Abstraction;
using Common.Messaging.Events;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace Common.RabbitMQ;

public class RabbitMQBus(IBus bus) : IEventBus
{
    public async Task PublishAsync<T>(T @event) where T : IntegrationEvent
    {
        // todo examine below. it has headers and extra stuff that might be useful
        // bus.Publish(@event, context =>
        // {
        //     
        // });
        await bus.Publish(@event);
    }
}