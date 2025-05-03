using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Common.Core.Models;
using Common.Logging.Events.Backend;
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
    IAnalysisExecutionRepository analysisExecutionRepository,
    ILogger<StopAnalysisExecutionHandler> logger,
    IMapper mapper
) : IRequestHandler<StopAnalysisExecutionRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(StopAnalysisExecutionRequest request, CancellationToken cancellationToken)
    {
        logger.LogDebug(AnalysisExecutionLogEvents.StopAnalysisExecution,
            "Validating stop analysis[{AnalysisExecution}] request", request.AnalysisExecutionId);
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        logger.LogInformation(AnalysisExecutionLogEvents.StopAnalysisExecution,
            "Stopping analysis execution> Fetching plugins of {AnalysisExecution}", request.AnalysisExecutionId);
        await analysisExecutionRepository.SetAnalysisExecutionProgress(request.AnalysisExecutionId, 100, 100);
        var executions = await pluginRepository.GetPluginOfAnalysis(request.AnalysisExecutionId);
        var @event = new StopAnalysisEvent
        {
            CreatedDate = DateTime.UtcNow,
            EventId = Guid.NewGuid(),
            PluginExecutionIds = executions.Select(s => s.Id).ToArray()
        };
        logger.LogInformation(
            "Sending stop event of analysis[{AnalysisExecution}] to message broker on PluginExecutionIds:{PluginExecutions}",
            request.AnalysisExecutionId, string.Join(",", @event.PluginExecutionIds));
        await messageBroker.PublishAsync(@event);
        return MethodResponse.Success("Done");
    }
}