using AutoMapper;
using Common.Logging.Events;
using Common.Messaging.Abstraction;
using Common.Messaging.Events.AnalysisExecution;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Worker.Application.Features.RunAnalysisRequested;

namespace Worker.Infrastructure.Messaging.Consumers;

public class RunAnalysisRequestedEventConsumer(
    ILogger<RunAnalysisRequestedEventConsumer> logger,
    IMapper mapper,
    IMediator mediator,
    IEventBus eventBus)
    : IConsumer<RunAnalysisRequestedEvent>
{
    public async Task Consume(ConsumeContext<RunAnalysisRequestedEvent> context)
    {
        logger.LogInformation(MQEvents.RunAnalysisRequestedEvent,
            "Run analysis event requested for Analysis {AnalysisExecution}, eventId:{TickerId} @ {TimeFrame}",
            context.Message.ExecutionId, context.Message.Ticker, context.Message.Timeframe);
        var message = mapper.Map<RunAnalysisRequest>(context.Message);
        var mr = await mediator.Send(message);
        if (mr.IsSuccess)
        {
            // await eventBus.PublishAsync(new PluginStatusEvent(message.ExecutionId, PluginStatus.Queued));
            return;
        }

        throw new NotImplementedException(mr.Message);
    }
}