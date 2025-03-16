using FluentValidation;

namespace Backend.Application.Features.Execution.CreateAnalysisExecution;

public class CreateAnalysisExecutionRequestValidator : AbstractValidator<CreateAnalysisExecutionRequest>
{
    public CreateAnalysisExecutionRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("CreateAnalysisExecutionRequest can't be null");
        RuleFor(f => f.Symbol).NotNull().WithMessage("CreateAnalysisExecutionRequest.Symbol can't be null")
            .MinimumLength(3).WithMessage("CreateAnalysisExecutionRequest.Symbol must be at least 3 chars")
            .MaximumLength(20).WithMessage("CreateAnalysisExecutionRequest.Symbol must be max 20 chars");
        RuleFor(f => f.Timeframe).IsInEnum().WithMessage("CreateAnalysisExecutionRequest.Timeframe is invalid");
        RuleFor(f => f.PluginIdentifier).NotNull().WithMessage("CreateAnalysisExecutionRequest.Identifier can't be null")
            .MinimumLength(5).WithMessage("CreateAnalysisExecutionRequest.Identifier must be at least 5 chars");
        RuleFor(f => f.StartDate).NotNull().WithMessage("CreateAnalysisExecutionRequest.StartDate can't be null")
            .NotEqual(default(DateTime)).WithMessage("CreateAnalysisExecutionRequest.StartDate can't be default");
    }
}