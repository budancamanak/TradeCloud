using FluentValidation;
using Market.Application.Features.GetPricesForPlugin.Request;

namespace Market.Application.Features.GetPricesForPlugin.Validator;

public class GetPricesForPluginQueryValidator : AbstractValidator<GetPricesForPluginQuery>
{
    public GetPricesForPluginQueryValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("GetPricesForPluginQuery can't be null");
        RuleFor(f => f.StartDate)
            .NotNull().WithMessage("Query start date can't be null")
            .NotEqual(default(DateTime)).WithMessage("Query start date can't be null");
        RuleFor(f => f.EndDate)
            .NotNull().WithMessage("Query end date can't be null")
            .NotEqual(default(DateTime)).WithMessage("Query end date can't be null");
        RuleFor(f => f.PluginId)
            .NotNull().WithMessage("Query pluginId can't be null")
            .GreaterThan(0).WithMessage("Query pluginId can't be lower than 0");
        RuleFor(f => f.TickerId)
            .NotEmpty().WithMessage("Query's ticker cannot be empty")
            .GreaterThan(0).WithMessage("Query's ticker cannot be lower than 1");
        RuleFor(f => f.Timeframe)
            .IsInEnum().WithMessage("Query's timeframe is not correct");
    }
}