using FluentValidation;
using Market.Application.Features.SaveFetchedPrices.Request;

namespace Market.Application.Features.SaveFetchedPrices.Validator;

public class PriceFetchCompletedValidator : AbstractValidator<PriceFetchCompletedCommand>
{
    public PriceFetchCompletedValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("Event can't be null");
        // todo might enable to load price data just to show on charts rather than having a mandatory plugin execution
        RuleFor(f => f.PluginId)
            .NotNull().WithMessage("Event pluginId can't be null")
            .GreaterThan(0).WithMessage("Event pluginId can't be lower than 0");
        RuleFor(f => f.TickerId)
            .NotEmpty().WithMessage("Event tickerId can't be null")
            .GreaterThan(0).WithMessage("Event ticker cannot be lower than 1");
        RuleFor(f => f.PriceInfo)
            .NotNull().WithMessage("Event price info can't be null")
            .NotEmpty().WithMessage("Event price info can't be empty");
    }
}