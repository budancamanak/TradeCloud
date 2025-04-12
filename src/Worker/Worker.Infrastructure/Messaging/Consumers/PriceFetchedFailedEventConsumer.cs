using MassTransit;

namespace Worker.Infrastructure.Messaging.Consumers;

public class PriceFetchedFailedEventConsumer : IConsumer<PriceFetchedFailedEventConsumer>
{
    public Task Consume(ConsumeContext<PriceFetchedFailedEventConsumer> context)
    {
        throw new NotImplementedException();
    }
}