using Common.Security.Enums;
using FluentValidation;

namespace Security.Application.Features.User.AddRoleToUser;

public class AddRoleToUserRequestValidator : AbstractValidator<AddRoleToUserRequest>
{
    public AddRoleToUserRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("AddRoleToUserRequest can't be null");
        RuleFor(f => f.UserId).GreaterThan(0).WithMessage("AddRoleToUserRequest.UserId can't be smaller than 1");
        RuleFor(f => f.RoleId).GreaterThan(0).WithMessage("AddRoleToUserRequest.RoleId can't be smaller than 1");
        RuleFor(f => f.RoleId).Custom(((i, context) =>
        {
            var role = Roles.FromValue(i);
            if (role != null) return;
            context.AddFailure(context.PropertyPath, "RoleId is not recognized");
        }));
    }
}