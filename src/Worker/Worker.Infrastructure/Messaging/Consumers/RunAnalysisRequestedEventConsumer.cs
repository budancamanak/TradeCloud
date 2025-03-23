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

public class RunAnalysisRequestedEventConsumer(IMapper mapper, IMediator mediator,IPublishEndpoint publishEndpoint, IEventBus eventBus)
    : IConsumer<RunAnalysisRequestedEvent>
{
    public async Task Consume(ConsumeContext<RunAnalysisRequestedEvent> context)
    {
        // var message = context.Message;
        var message = mapper.Map<RunPluginRequest>(context.Message);
        var mr = await mediator.Send(message);
        if (mr.IsSuccess)
        {
            // await eventBus.PublishAsync(new PluginStatusEvent(message.ExecutionId, PluginStatus.Queued));
            return;
        }
        
        /***
         * this will be run analysis requested event consumer
         * 
         * .
         * 
         */


        throw new NotImplementedException(mr.Message);
    }
}