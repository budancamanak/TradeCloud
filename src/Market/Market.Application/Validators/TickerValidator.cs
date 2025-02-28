using FluentValidation;
using Market.Domain.Entities;

namespace Market.Application.Validators;

public class TickerValidator:AbstractValidator<Ticker>
{
    public TickerValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("Ticker can't be null");
        RuleFor(f => f.ExchangeId).NotEmpty().WithMessage("ExchangeId cannot be empty")
            .GreaterThan(0).WithMessage("ExchangeId cannot be lower than 1");
        RuleFor(f => f.Symbol).NotEmpty().WithMessage("Ticker's symbol cannot be empty")
            .MaximumLength(25).WithMessage("Ticker's symbol cannot be longer than 25")
            .MinimumLength(2).WithMessage("Ticker's symbol cannot be shorter than 2");
        RuleFor(f => f.Name).NotEmpty().WithMessage("Ticker's name cannot be empty")
            .MaximumLength(50).WithMessage("Ticker's name cannot be longer than 50")
            .MinimumLength(2).WithMessage("Ticker's name cannot be shorter than 2");
        RuleFor(f => f.DecimalPoint).NotEmpty().WithMessage("Ticker's decimal point cannot be empty")
            .InclusiveBetween(1, 10).WithMessage("Ticker's decimal point must be between 1-10");
    }
}