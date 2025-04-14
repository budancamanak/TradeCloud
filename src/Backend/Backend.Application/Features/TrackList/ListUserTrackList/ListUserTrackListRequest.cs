using Common.Core.DTOs.Backend;
using MediatR;

namespace Backend.Application.Features.TrackList.ListUserTrackList;

public class ListUserTrackListRequest : IRequest<List<TrackListDto>>
{
    public int UserId { get; set; }
}