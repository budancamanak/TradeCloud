using FluentValidation;

namespace Backend.Application.Features.TrackList.ListAvailableTickers;

public class ListAvailableTickersRequestValidator : AbstractValidator<ListAvailableTickersRequest>
{
    public ListAvailableTickersRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("List AvailableTickers Request can't be null");
    }
}