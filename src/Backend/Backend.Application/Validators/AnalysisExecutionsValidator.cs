using Backend.Domain.Entities;
using FluentValidation;

namespace Backend.Application.Validators;

public class AnalysisExecutionsValidator : AbstractValidator<AnalysisExecution>
{
    public AnalysisExecutionsValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("AnalysisExecution object can't be null");
        RuleFor(f => f.Timeframe).IsInEnum().WithMessage("AnalysisExecution's timeframe wrong");
        RuleFor(f => f.PluginIdentifier).NotNull().WithMessage("AnalysisExecution's plugin identifier can't be null")
            .NotEmpty().WithMessage("AnalysisExecution's plugin identifier can't be empty");
        RuleFor(f => f.ParamSet).NotNull().WithMessage("AnalysisExecution's param set can't be null")
            .NotEmpty().WithMessage("AnalysisExecution's param set can't be empty");
        RuleFor(f => f.StartDate).NotNull().WithMessage("AnalysisExecution.StartDate can't be null")
            .NotEqual(default(DateTime)).WithMessage("AnalysisExecution.StartDate can't be default");
        RuleFor(f => f.EndDate).NotNull().WithMessage("AnalysisExecution.StartDate can't be null")
            .NotEqual(default(DateTime)).WithMessage("AnalysisExecution.StartDate can't be default");
        RuleFor(f => f.TickerId).GreaterThan(0).WithMessage("AnalysisExecution.ticker can't be lower than 1");
    }
}