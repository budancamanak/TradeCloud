using FluentValidation;

namespace Backend.Application.Features.TrackList.ListUserTrackList;

public class ListUserTrackListRequestValidator : AbstractValidator<ListUserTrackListRequest>
{
    public ListUserTrackListRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("TrackList object can't be null");
        RuleFor(f => f.UserId)
            .NotNull().WithMessage("ListUserTrackListRequest.UserId can't be null")
            .GreaterThan(0).WithMessage("ListUserTrackListRequest.UserId can't be lower than 1");
    }
}