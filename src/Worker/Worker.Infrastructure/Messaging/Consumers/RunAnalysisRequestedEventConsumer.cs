﻿using AutoMapper;
using Common.Messaging.Abstraction;
using Common.Messaging.Events.AnalysisExecution;
using MassTransit;
using MediatR;
using Worker.Application.Features.RunAnalysisRequested;

namespace Worker.Infrastructure.Messaging.Consumers;

public class RunAnalysisRequestedEventConsumer(
    IMapper mapper,
    IMediator mediator,
    IPublishEndpoint publishEndpoint,
    IEventBus eventBus)
    : IConsumer<RunAnalysisRequestedEvent>
{
    public async Task Consume(ConsumeContext<RunAnalysisRequestedEvent> context)
    {
        // var message = context.Message;
        var message = mapper.Map<RunAnalysisRequest>(context.Message);
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