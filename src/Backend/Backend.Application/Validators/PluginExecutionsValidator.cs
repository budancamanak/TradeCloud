using Backend.Domain.Entities;
using FluentValidation;

namespace Backend.Application.Validators;

public class PluginExecutionsValidator : AbstractValidator<PluginExecution>
{
    public PluginExecutionsValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("PluginExecutions object can't be null");
        RuleFor(f => f.TickerId)
            .NotNull().WithMessage("PluginExecutions.TickerId can't be null")
            .GreaterThan(0).WithMessage("PluginExecutions.TickerId can't be lower than 1");
        RuleFor(f => f.StartDate)
            .NotNull().WithMessage("PluginExecutions start date can't be null")
            .NotEqual(default(DateTime)).WithMessage("PluginExecutions start date can't be default");
        RuleFor(f => f.PluginIdentifier)
            .NotEmpty().WithMessage("PluginExecutions plugin identifier can't be empty")
            .MinimumLength(5).WithMessage("PluginExecutions plugin identifier can't be shorter than 5");
        RuleFor(f => f.Timeframe)
            .NotNull().WithMessage("PluginExecutions timeframe can't be null")
            .IsInEnum().WithMessage("PluginExecutions timeframe is incorrect");
    }
}