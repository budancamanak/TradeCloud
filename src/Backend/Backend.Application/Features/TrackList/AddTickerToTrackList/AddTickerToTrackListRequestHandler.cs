using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Common.Core.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.TrackList.AddTickerToTrackList;

public class AddTickerToTrackListRequestHandler(
    ITrackListRepository repository,
    IValidator<AddTickerToTrackListRequest> validator,
    IMapper mapper,
    ILogger<AddTickerToTrackListRequestHandler> logger)
    : IRequestHandler<AddTickerToTrackListRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(AddTickerToTrackListRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var item = mapper.Map<Domain.Entities.TrackList>(request);
        var mr = await repository.AddAsync(item);
        return mr;
    }
}