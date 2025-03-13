using Ardalis.GuardClauses;
using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Backend.Application.Features.Execution.CreatePluginExecution;
using Backend.Domain.Entities;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Web.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.Execution.CreateAnalysisExecution;

public class CreateAnalysisExecutionRequestHandler(
    IValidator<CreateAnalysisExecutionRequest> validator,
    IMapper mapper,
    ITickerService tickerService,
    IPluginService pluginService,
    IAnalysisExecutionRepository repository,
    ILogger<CreatePluginExecutionRequestHandler> logger)
    : IRequestHandler<CreateAnalysisExecutionRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(CreateAnalysisExecutionRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var ticker = await tickerService.GetTickerWithSymbol(request.Symbol);
        Guard.Against.Null(ticker, exceptionCreator: () => new RequestValidationException("Failed to find ticker"));
        var plugin = await pluginService.GetPluginInfo(request.PluginIdentifier);
        Guard.Against.Null(plugin, exceptionCreator: () => new RequestValidationException("Failed to find plugin"));
        var item = mapper.Map<CreateAnalysisExecutionRequest, AnalysisExecution>(request,
            opts =>
            {
                // todo use Logged User Id
                opts.Items["CurrentUserId"] = 1;
                opts.Items["TickerId"] = ticker.Id;
                opts.Items["PluginName"] = plugin.Name;
            }
        );
        Guard.Against.Null(item, message: "Request mapping failed");
        logger.LogInformation("Creating analysis execution for [{}] in request handler. For {} @ {}",
            item.PluginIdentifier, request.Symbol, request.Timeframe.GetStringRepresentation());
        var mr = await repository.AddAsync(item);
        return mr;
    }
}