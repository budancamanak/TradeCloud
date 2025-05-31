using Ardalis.GuardClauses;
using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Backend.Domain.Entities;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Logging.Events.Backend;
using Common.Messaging.Abstraction;
using Common.Messaging.Events.AnalysisExecution;
using Common.Web.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.Execution.CreateAnalysisExecution;

public class CreateAnalysisExecutionRequestHandler(
    IValidator<CreateAnalysisExecutionRequest> validator,
    IMapper mapper,
    ITickerService tickerService,
    IEventBus messageBroker,
    IPluginService pluginService,
    IAnalysisExecutionRepository repository,
    IPluginExecutionRepository pluginRepository,
    IPluginExecutionEngine pluginExecutionEngine,
    ILogger<CreateAnalysisExecutionRequestHandler> logger)
    : IRequestHandler<CreateAnalysisExecutionRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(CreateAnalysisExecutionRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var ticker = await tickerService.GetTickerWithSymbol(request.Symbol);
        Guard.Against.Null(ticker,
            exceptionCreator: () => new RequestValidationException($"Failed to find ticker {request.Symbol}"));
        var plugin = await pluginService.GetPluginInfo(request.PluginIdentifier);
        Guard.Against.Null(plugin,
            exceptionCreator: () =>
                new RequestValidationException($"Failed to find plugin {request.PluginIdentifier}"));
        var analysisExecution = mapper.Map<CreateAnalysisExecutionRequest, AnalysisExecution>(request,
            opts =>
            {
                // todo use Logged User Id
                opts.Items["CurrentUserId"] = 1;
                opts.Items["TickerId"] = ticker.Id;
                opts.Items["PluginName"] = plugin.Name;
            }
        );
        Guard.Against.Null(analysisExecution, message: "Request mapping failed");
        logger.LogInformation(AnalysisExecutionLogEvents.CreateAnalysisExecution,
            "Creating analysis execution for [{Identifier}] in request handler. For {Symbol} @ {Timeframe}",
            analysisExecution.PluginIdentifier, request.Symbol, request.Timeframe.GetStringRepresentation());
        var executions = pluginExecutionEngine.GeneratePluginExecutions(analysisExecution);
        executions.ForEach(f => analysisExecution.PluginExecutions.Add(f));
        var mr = await repository.AddAsync(analysisExecution);
        if (mr.IsSuccess)
        {
            // var executions = pluginExecutionEngine.GeneratePluginExecutions(analysisExecution);
            // int savedCount = 0, failedCount = 0;
            // foreach (var item in executions)
            // {
            //     try
            //     {
            //         mr = await pluginRepository.AddAsync(item);
            //         if (!mr.IsSuccess)
            //         {
            //             logger.LogCritical(AnalysisExecutionLogEvents.CreateAnalysisExecution,
            //                 "Failed to save plugin execution!! for : {PluginExecution}", item);
            //             failedCount++;
            //         }
            //         else
            //         {
            //             savedCount++;
            //         }
            //     }
            //     catch (AlreadySavedException e)
            //     {
            //         logger.LogDebug(AnalysisExecutionLogEvents.CreateAnalysisExecution,
            //             "Plugin[{PluginExecution}] is already saved. Safe exception skip. {Exception}", item, e);
            //         // pass.
            //         savedCount++;
            //     }
            // }
            //
            // if (failedCount == executions.Count)
            // {
            //     logger.LogCritical(AnalysisExecutionLogEvents.CreateAnalysisExecution,
            //         "Failed to create plugin executions for analysis[{AnalysisExecution}]", mr.Id);
            //     await repository.SetAnalysisExecutionProgress(mr.Id, 1, 1);
            //     var failEvent = new AnalysisFinishedEvent();
            //     await messageBroker.PublishAsync(failEvent);
            //     return MethodResponse.Error(
            //         $"Failed to create plugin executions for analysis[{mr.Id}]. Stopped execution");
            // }
            //
            // await repository.SetAnalysisExecutionProgress(mr.Id, 0, savedCount);
            //
            logger.LogInformation(AnalysisExecutionLogEvents.CreateAnalysisExecution,
                "Created analysis execution for [{Identifier}] in request handler. For {Symbol} @ {Timeframe}",
                analysisExecution.PluginIdentifier, request.Symbol, request.Timeframe.GetStringRepresentation());
        }
        else
            logger.LogInformation(AnalysisExecutionLogEvents.CreateAnalysisExecution,
                "Failed to create analysis execution for [{Identifier}] in request handler. For {Symbol} @ {Timeframe}. Reason: {Reason}",
                analysisExecution.PluginIdentifier, request.Symbol, request.Timeframe.GetStringRepresentation(),
                mr.Message);

        return mr;
    }
}