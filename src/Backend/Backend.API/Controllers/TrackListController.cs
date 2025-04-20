using Backend.Application.Features.TrackList.AddTickerToTrackList;
using Backend.Application.Features.TrackList.ListAvailableTickers;
using Backend.Application.Features.TrackList.ListUserTrackList;
using Backend.Application.Features.TrackList.RemoveUserTrackList;
using Common.Core.DTOs;
using Common.Core.DTOs.Backend;
using Common.Core.Models;
using Common.Security.Attributes;
using Common.Web.Attributes.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[HasScope("Level1")]
[ApiController]
[Route("[controller]")]
public class TrackListController(
    ILogger<TrackListController> logger,
    IMediator mediator)
{
    [HttpGet("/AvailableTickers")]
    [HasPolicy(Policies.TrackList.Read)]
    public async Task<List<TickerDto>> GetAvailableTickers()
    {
        var request = new ListAvailableTickersRequest();
        var result = await mediator.Send(request);
        return result;
    }

    [HttpGet]
    [HasPolicy(Policies.TrackList.Read)]
    public async Task<List<TrackListDto>> GetUserTrackList([FromQuery] ListUserTrackListRequest request)
    {
        var result = await mediator.Send(request);
        return result;
    }

    [HttpPost]
    [HasPolicy(Policies.TrackList.Write)]
    public async Task<MethodResponse> AddTickerToUserTrackList([FromBody] AddTickerToTrackListRequest request)
    {
        var result = await mediator.Send(request);
        return result;
    }

    [HttpDelete("User/{userId:int}/Ticker/{tickerId:int}")]
    [HasPolicy(Policies.TrackList.Write)]
    public async Task<MethodResponse> RemoveTickerFromUserTrackList(int userId, int tickerId)
    {
        var request = new RemoveUserTrackListRequest { UserId = userId, TickerId = tickerId };
        var mr = await mediator.Send(request);
        return mr;
    }
}