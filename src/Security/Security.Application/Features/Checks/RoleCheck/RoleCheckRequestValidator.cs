using Common.Core.Models;
using FluentValidation;

namespace Security.Application.Features.Checks.RoleCheck;

public class RoleCheckRequestValidator : BaseCheckRequestValidator<RoleCheckRequest, MethodResponse>
{
    public RoleCheckRequestValidator() : base()
    {
        RuleFor(f => f.Role)
            .NotNull().WithMessage("CheckRequest.Role can't be null")
            .NotEmpty().WithMessage("CheckRequest.Role can't be empty");
    }
}