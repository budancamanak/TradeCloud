using FluentValidation;

namespace Backend.Application.Features.Execution.CreateAnalysisExecution;

public class CreateAnalysisExecutionRequestValidator : AbstractValidator<CreateAnalysisExecutionRequest>
{
    public CreateAnalysisExecutionRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("CreatePluginExecutionRequest can't be null");
        RuleFor(f => f.Symbol).NotNull().WithMessage("CreatePluginExecutionRequest.Symbol can't be null")
            .MinimumLength(3).WithMessage("CreatePluginExecutionRequest.Symbol must be at least 3 chars")
            .MaximumLength(20).WithMessage("CreatePluginExecutionRequest.Symbol must be max 20 chars");
        RuleFor(f => f.Timeframe).IsInEnum().WithMessage("CreatePluginExecutionRequest.Timeframe is invalid");
        RuleFor(f => f.PluginIdentifier).NotNull().WithMessage("CreatePluginExecutionRequest.Identifier can't be null")
            .MinimumLength(5).WithMessage("CreatePluginExecutionRequest.Identifier must be at least 5 chars");
        RuleFor(f => f.StartDate).NotNull().WithMessage("CreatePluginExecutionRequest.StartDate can't be null")
            .NotEqual(default(DateTime)).WithMessage("CreatePluginExecutionRequest.StartDate can't be default");
    }
}