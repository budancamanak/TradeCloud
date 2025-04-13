using Backend.Application.Features.TrackList.ListAvailableTickers;
using Common.Core.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TrackListController(
    ILogger<TrackListController> logger,
    IMediator mediator)
{
    [HttpGet("GetAvailableTickers")]
    public async Task<List<TickerDto>> GetAvailablePlugins()
    {
        var request = new ListAvailableTickersRequest();
        var result = await mediator.Send(request);
        return result;
    }
}