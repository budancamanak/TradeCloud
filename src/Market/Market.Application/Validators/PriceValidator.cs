using FluentValidation;
using Market.Domain.Entities;

namespace Market.Application.Validators;

public class PriceValidator : AbstractValidator<Price>
{
    public PriceValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("Price can't be null");
        RuleFor(f => f.Timestamp).NotNull().WithMessage("Price's timestamp can't be null");
        RuleFor(f => f.TickerId).NotEmpty().WithMessage("Price's ticker cannot be empty")
            .GreaterThan(0).WithMessage("Price's ticker cannot be lower than 1");
        RuleFor(f => f.High).NotEmpty().WithMessage("Price's high cannot be empty")
            .GreaterThan(0).WithMessage("Price's high cannot be lower than 1");
        RuleFor(f => f.Low).NotEmpty().WithMessage("Price's Low cannot be empty")
            .GreaterThan(0).WithMessage("Price's Low cannot be lower than 1");
        RuleFor(f => f.Close).NotEmpty().WithMessage("Price's Close cannot be empty")
            .GreaterThan(0).WithMessage("Price's Close cannot be lower than 1");
        RuleFor(f => f.Open).NotEmpty().WithMessage("Price's Open cannot be empty")
            .GreaterThan(0).WithMessage("Price's Open cannot be lower than 1");
    }
}