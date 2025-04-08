using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Common.Core.Models;
using Common.Messaging.Abstraction;
using Common.Messaging.Events.AnalysisExecution;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.Execution.StopAnalysisExecution;

public class StopAnalysisExecutionHandler(
    IValidator<StopAnalysisExecutionRequest> validator,
    IEventBus messageBroker,
    IPluginExecutionRepository pluginRepository,
    ILogger<StopAnalysisExecutionHandler> logger,
    IMapper mapper
) : IRequestHandler<StopAnalysisExecutionRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(StopAnalysisExecutionRequest request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Validating  stop analysis request");
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        logger.LogInformation("StopAnalysisExecutionHandler> Fetching plugins of {}", request.AnalysisExecutionId);
        var executions = await pluginRepository.GetPluginOfAnalysis(request.AnalysisExecutionId);
        var @event = new StopAnalysisEvent
        {
            CreatedDate = DateTime.UtcNow,
            EventId = Guid.NewGuid(),
            PluginExecutionIds = executions.Select(s => s.Id).ToArray()
        };
        logger.LogInformation("Sending stop event to message broker on PluginExecutionIds:{}",
            string.Join(",", @event.PluginExecutionIds));
        await messageBroker.PublishAsync(@event);
        return MethodResponse.Success("Done");
    }
}