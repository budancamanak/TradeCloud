using AutoMapper;
using Common.Core.Enums;
using Common.Grpc;
using Common.Messaging.Abstraction;
using Common.Messaging.Events.PluginExecution;
using Google.Protobuf.WellKnownTypes;
using MassTransit;
using MediatR;
using Worker.Application.Abstraction;
using Worker.Application.Features.RunPluginRequested;

namespace Worker.Infrastructure.Messaging.Consumers;

public class RunPluginRequestedEventConsumer(IMapper mapper, IMediator mediator,IPublishEndpoint publishEndpoint, IEventBus eventBus)
    : IConsumer<RunPluginRequestedEvent>
{
    public async Task Consume(ConsumeContext<RunPluginRequestedEvent> context)
    {
        // var message = context.Message;
        var message = mapper.Map<RunPluginRequest>(context.Message);
        var mr = await mediator.Send(message);
        if (mr.IsSuccess)
        {
            // await eventBus.PublishAsync(new PluginStatusEvent(message.ExecutionId, PluginStatus.Queued));
            return;
        }


        throw new NotImplementedException(mr.Message);
    }
}