using Backend.Domain.Entities;
using FluentValidation;

namespace Backend.Application.Validators;

public class TrackListValidator : AbstractValidator<TrackList>
{
    public TrackListValidator()
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