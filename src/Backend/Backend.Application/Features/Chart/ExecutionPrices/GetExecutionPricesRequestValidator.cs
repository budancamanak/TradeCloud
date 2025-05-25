using FluentValidation;

namespace Backend.Application.Features.Chart.ExecutionPrices;

public class GetExecutionPricesRequestValidator : AbstractValidator<GetExecutionPricesRequest>
{
    public GetExecutionPricesRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("ExecutionPricesRequest request can't be null");
        RuleFor(f => f.ExecutionId).GreaterThan(0)
            .WithMessage("ExecutionPricesRequest.ExecutionId can't be lower than 1");
    }
}