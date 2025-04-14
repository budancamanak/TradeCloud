using Backend.Application.Features.TrackList.AddTickerToTrackList;
using Backend.Application.Features.TrackList.ListAvailableTickers;
using Backend.Application.Features.TrackList.ListUserTrackList;
using Common.Core.DTOs;
using Common.Core.DTOs.Backend;
using Common.Core.Models;
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
    public async Task<List<TickerDto>> GetAvailableTickers()
    {
        var request = new ListAvailableTickersRequest();
        var result = await mediator.Send(request);
        return result;
    }

    [HttpGet("GetUserTrackList")]
    public async Task<List<TrackListDto>> GetUserTrackList([FromQuery] ListUserTrackListRequest request)
    {
        var result = await mediator.Send(request);
        return result;
    }

    [HttpPost("AddTickerToUserTrackList")]
    public async Task<MethodResponse> AddTickerToUserTrackList([FromBody] AddTickerToTrackListRequest request)
    {
        var result = await mediator.Send(request);
        return result;
    }
}