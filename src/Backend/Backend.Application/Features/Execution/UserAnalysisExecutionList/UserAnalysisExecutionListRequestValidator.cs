using FluentValidation;

namespace Backend.Application.Features.Execution.UserAnalysisExecutionList;

public class UserAnalysisExecutionListRequestValidator : AbstractValidator<UserAnalysisExecutionListRequest>
{
    public UserAnalysisExecutionListRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("AnalysisExecutionDetailsRequest request can't be null");
        RuleFor(f => f.UserId).GreaterThan(0)
            .WithMessage("UserAnalysisExecutionListRequestValidator.UserId can't be lower than 1");
        RuleFor(f => f.Status).IsInEnum()
            .WithMessage("UserAnalysisExecutionListRequestValidator.Status is invalid");
    }
}