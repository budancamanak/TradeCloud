using FluentValidation;

namespace Backend.Application.Features.Execution.StopAnalysisExecution;

public class StopAnalysisExecutionValidator: AbstractValidator<StopAnalysisExecutionRequest>
{
    public StopAnalysisExecutionValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("StopAnalysisExecutionRequest can't be null");
        RuleFor(f => f.AnalysisExecutionId).NotNull().WithMessage("StopAnalysisExecutionRequest.ExecutionId can't be null")
            .GreaterThan(0).WithMessage("StopAnalysisExecutionRequest.ExecutionId must be positive");
    }
}