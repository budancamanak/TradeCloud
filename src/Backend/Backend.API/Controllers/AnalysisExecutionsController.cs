using AutoMapper;
using Backend.API.Models;
using Backend.Application.Features.Execution.CreateAnalysisExecution;
using Backend.Application.Features.Execution.ListActivePlugins;
using Backend.Application.Features.Execution.ListAvailablePlugins;
using Backend.Application.Features.Execution.RunAnalysisExecution;
using Backend.Application.Features.Execution.StopAnalysisExecution;
using Common.Core.DTOs.Backend;
using Common.Core.Models;
using Common.Plugin.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AnalysisExecutionsController(
    ILogger<AnalysisExecutionsController> logger,
    IMediator mediator,
    IMapper mapper)
{
    [HttpGet("GetAvailablePlugins")]
    public async Task<List<IPlugin.PluginInfo>> GetAvailablePlugins()
    {
        var request = new ListAvailablePluginsRequest();
        var result = await mediator.Send(request);
        return result;
    }

    [HttpGet("ActivePlugins")]
    public async Task<List<PluginExecutionsDto>> GetActivePlugins([FromQuery] ListActivePluginsRequest request)
    {
        var result = await mediator.Send(request);
        return result;
    }

    [HttpPost]
    public async Task<MethodResponse> CreateAnalysisExecution([FromBody] CreateAnalysisExecutionModel model)
    {
        var request = mapper.Map<CreateAnalysisExecutionRequest>(model);
        var result = await mediator.Send(request);
        return result;
    }

    [HttpPatch]
    public async Task<MethodResponse> RunAnalysisExecution([FromBody] RunAnalysisExecutionRequest request)
    {
        var result = await mediator.Send(request);
        return result;
    }

    [HttpDelete]
    public async Task<MethodResponse> StopAnalysisExecution([FromBody] StopAnalysisExecutionRequest request)
    {
        var result = await mediator.Send(request);
        return result;
    }
}