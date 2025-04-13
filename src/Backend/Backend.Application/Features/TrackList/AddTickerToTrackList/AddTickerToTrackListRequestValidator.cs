using FluentValidation;

namespace Backend.Application.Features.TrackList.AddTickerToTrackList;

public class AddTickerToTrackListRequestValidator : AbstractValidator<AddTickerToTrackListRequest>
{
    public AddTickerToTrackListRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("TrackList object can't be null");
        RuleFor(f => f.TickerId)
            .NotNull().WithMessage("TrackList.TickerId can't be null")
            .GreaterThan(0).WithMessage("TrackList.TickerId can't be lower than 1");
        RuleFor(f => f.UserId)
            .NotNull().WithMessage("TrackList.UserId can't be null")
            .GreaterThan(0).WithMessage("TrackList.UserId can't be lower than 1");
    }
}