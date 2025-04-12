using Backend.Application.Features.Execution.CreateAnalysisExecution;
using Backend.Application.Features.Execution.CreatePluginExecution;
using Backend.Application.Features.Execution.ListActivePlugins;
using Backend.Application.Features.Execution.ListAvailablePlugins;
using Backend.Application.Features.Execution.RunPluginExecution;
using Common.Core.DTOs.Backend;
using Common.Core.Models;
using Common.Plugin.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PluginExecutionsController(ILogger<PluginExecutionsController> logger, IMediator mediator)
{
    [HttpGet("GetAvailablePlugins")]
    public async Task<List<IPlugin.PluginInfo>> GetAvailablePlugins()
    {
        var request = new ListAvailablePluginsRequest();
        var result = await mediator.Send(request);
        return result;
    }

    [HttpGet("ActivePlugins")]
    public async Task<List<PluginExecutionsDto>> GetActivePlugins()
    {
        var request = new ListActivePluginsRequest();
        var result = await mediator.Send(request);
        return result;
    }
}