using FluentValidation;
using Market.Domain.Entities;

namespace Market.Application.Validators;

public class ExchangeValidator : AbstractValidator<Exchange>
{
    public ExchangeValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("Exchange can't be null");
        RuleFor(f => f.Name).NotEmpty().WithMessage("Exchange's name cannot be empty")
            .MaximumLength(50).WithMessage("Exchange's name cannot be longer than 50")
            .MinimumLength(2).WithMessage("Exchange's name cannot be shorter than 2");
    }
}