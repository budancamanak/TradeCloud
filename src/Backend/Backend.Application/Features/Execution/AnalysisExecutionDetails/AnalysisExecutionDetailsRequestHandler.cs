using Ardalis.GuardClauses;
using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Common.Core.DTOs.Backend;
using Common.Core.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.Execution.AnalysisExecutionDetails;

public class AnalysisExecutionDetailsRequestHandler(
    IValidator<AnalysisExecutionDetailsRequest> validator,
    IAnalysisExecutionRepository analysisExecutionRepository,
    IPluginExecutionRepository pluginExecutionRepository,
    IPluginOutputRepository pluginOutputRepository,
    IMapper mapper,
    ILogger<AnalysisExecutionDetailsRequestHandler> logger,
    IPluginService pluginService)
    : IRequestHandler<AnalysisExecutionDetailsRequest, AnalysisExecutionDto>
{
    public async Task<AnalysisExecutionDto> Handle(AnalysisExecutionDetailsRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        logger.LogWarning("Getting execution details : {AnalysisExecutionId}", request.AnalysisExecutionId);
        var analysis = await analysisExecutionRepository.GetByIdAsync(request.AnalysisExecutionId);
        Guard.Against.Null(analysis);
        var pluginInfo = await pluginService.GetPluginInfo(analysis.PluginIdentifier);
        Guard.Against.Null(pluginInfo);

        var result = new AnalysisExecutionDto
        {
            Id = analysis.Id,
            PluginInfo = pluginInfo,
            Status = analysis.Status.GetStringRepresentation(),
            EndDate = analysis.EndDate,
            StartDate = analysis.StartDate,
            PluginExecutions = mapper.Map<List<PluginExecutionsDto>>(analysis.PluginExecutions).ToArray()
        };

        foreach (var item in result.PluginExecutions)
        {
            var outputs = await pluginOutputRepository.GetPluginOutputs(item.Id);
            var outputDtos =
                mapper.Map<List<PluginOutputDto>>(outputs, opts => { opts.Items["PluginName"] = pluginInfo.Name; });
            item.Outputs = outputDtos.ToArray();
        }

        return result;
    }
}