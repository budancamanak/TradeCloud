using FluentValidation;

namespace Backend.Application.Features.Execution.AnalysisExecutionDetails;

public class AnalysisExecutionDetailsRequestValidator : AbstractValidator<AnalysisExecutionDetailsRequest>
{
    public AnalysisExecutionDetailsRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("AnalysisExecutionDetailsRequest request can't be null");
        RuleFor(f => f.AnalysisExecutionId).GreaterThan(0)
            .WithMessage("AnalysisExecutionDetailsRequest.ExecutionId can't be lower than 1");
    }
}