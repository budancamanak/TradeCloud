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
using Common.Web.Http;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.Execution.CreateAnalysisExecution;

public class CreateAnalysisExecutionRequestHandler(
    IValidator<CreateAnalysisExecutionRequest> validator,
    IMapper mapper,
    ITickerService tickerService,
    IHttpContextAccessor contextAccessor,
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
                opts.Items["CurrentUserId"] = contextAccessor.CurrentUserId();
                opts.Items["TickerId"] = ticker.Id;
                opts.Items["PluginName"] = plugin.Name;
            }
        );
        Guard.Against.Null(analysisExecution, message: "Request mapping failed");
        logger.LogInformation(AnalysisExecutionLogEvents.CreateAnalysisExecution,
            "Creating analysis execution for [{Identifier}] in request handler. For {Symbol} @ {Timeframe}",
            analysisExecution.PluginIdentifier, request.Symbol, request.Timeframe.GetStringRepresentation());
        try
        {
            var executions = pluginExecutionEngine.GeneratePluginExecutionsLazy(analysisExecution);
            executions.ForEach(f => analysisExecution.PluginExecutions.Add(f));
            var mr = await repository.AddAsync(analysisExecution);
            if (mr.IsSuccess)
            {
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
        catch (InvalidOperationException ex)
        {
            logger.LogError(AnalysisExecutionLogEvents.CreateAnalysisExecution,
                "Failed to create analysis execution for [{Identifier}] in request handler. For {Symbol} @ {Timeframe}. Reason: {Reason}",
                analysisExecution.PluginIdentifier, request.Symbol, request.Timeframe.GetStringRepresentation(),
                ex.Message);
            return MethodResponse.Error(ex);
        }
    }
}