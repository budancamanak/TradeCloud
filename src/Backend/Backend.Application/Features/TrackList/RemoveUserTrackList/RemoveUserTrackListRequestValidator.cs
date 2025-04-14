using FluentValidation;

namespace Backend.Application.Features.TrackList.RemoveUserTrackList;

public class RemoveUserTrackListRequestValidator : AbstractValidator<RemoveUserTrackListRequest>
{
    public RemoveUserTrackListRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("RemoveUserTrackListRequest object can't be null");
        RuleFor(f => f.TickerId)
            .NotNull().WithMessage("RemoveUserTrackListRequest.TickerId can't be null")
            .GreaterThan(0).WithMessage("RemoveUserTrackListRequest.TickerId can't be lower than 1");
        RuleFor(f => f.UserId)
            .NotNull().WithMessage("RemoveUserTrackListRequest.UserId can't be null")
            .GreaterThan(0).WithMessage("RemoveUserTrackListRequest.UserId can't be lower than 1");
    }
}