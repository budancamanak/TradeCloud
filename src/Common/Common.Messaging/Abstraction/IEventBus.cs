using Common.Messaging.Events;

namespace Common.Messaging.Abstraction;

public interface IEventBus
{
    Task PublishAsync<T>(T @event) where T : IntegrationEvent;
}