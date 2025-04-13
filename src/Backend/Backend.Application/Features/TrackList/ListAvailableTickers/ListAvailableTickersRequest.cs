using Common.Core.DTOs;
using MediatR;

namespace Backend.Application.Features.TrackList.ListAvailableTickers;

public class ListAvailableTickersRequest : IRequest<List<TickerDto>>
{
}