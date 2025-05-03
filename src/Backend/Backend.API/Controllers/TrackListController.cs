using Backend.Application.Features.TrackList.AddTickerToTrackList;
using Backend.Application.Features.TrackList.ListAvailableTickers;
using Backend.Application.Features.TrackList.ListUserTrackList;
using Backend.Application.Features.TrackList.RemoveUserTrackList;
using Common.Core.DTOs;
using Common.Core.DTOs.Backend;
using Common.Core.Models;
using Common.Security.Attributes;
using Common.Security.Enums;
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
    [HasPermission(Permissions.Enum.ManageTrackList)]
    [HasRole(Roles.Enum.Viewer)]
    public async Task<List<TickerDto>> GetAvailableTickers()
    {
        var request = new ListAvailableTickersRequest();
        var result = await mediator.Send(request);
        return result;
    }

    [HttpGet]
    [HasPermission(Permissions.Enum.ManageTrackList)]
    [HasRole(Roles.Enum.Viewer)]
    public async Task<List<TrackListDto>> GetUserTrackList([FromQuery] ListUserTrackListRequest request)
    {
        var result = await mediator.Send(request);
        return result;
    }

    [HttpPost]
    [HasPermission(Permissions.Enum.ManageTrackList)]
    [HasRole(Roles.Enum.Viewer)]
    public async Task<MethodResponse> AddTickerToUserTrackList([FromBody] AddTickerToTrackListRequest request)
    {
        var result = await mediator.Send(request);
        return result;
    }

    [HttpDelete("User/{userId:int}/Ticker/{tickerId:int}")]
    [HasPermission(Permissions.Enum.ManageTrackList)]
    [HasRole(Roles.Enum.Viewer)]
    public async Task<MethodResponse> RemoveTickerFromUserTrackList(int userId, int tickerId)
    {
        var request = new RemoveUserTrackListRequest { UserId = userId, TickerId = tickerId };
        var mr = await mediator.Send(request);
        return mr;
    }
}