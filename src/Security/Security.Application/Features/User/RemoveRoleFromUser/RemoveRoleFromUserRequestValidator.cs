using Common.Security.Enums;
using FluentValidation;

namespace Security.Application.Features.User.RemoveRoleFromUser;

public class RemoveRoleFromUserRequestValidator : AbstractValidator<RemoveRoleFromUserRequest>
{
    public RemoveRoleFromUserRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("RemoveRoleFromUserRequest can't be null");
        RuleFor(f => f.UserId).GreaterThan(0).WithMessage("RemoveRoleFromUserRequest.UserId can't be smaller than 1");
        RuleFor(f => f.RoleId).GreaterThan(0).WithMessage("RemoveRoleFromUserRequest.RoleId can't be smaller than 1");
        RuleFor(f => f.RoleId).Custom(((i, context) =>
        {
            var role = Roles.FromValue(i);
            if (role != null) return;
            context.AddFailure(context.PropertyPath, "RemoveRoleFromUserRequest.RoleId is not recognized");
        }));
    }
}