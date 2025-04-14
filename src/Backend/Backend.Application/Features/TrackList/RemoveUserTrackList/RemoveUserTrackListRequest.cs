using Common.Core.Models;
using MediatR;

namespace Backend.Application.Features.TrackList.RemoveUserTrackList;

public class RemoveUserTrackListRequest : IRequest<MethodResponse>
{
    public int UserId { get; set; }
    public int TickerId { get; set; }
}