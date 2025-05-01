using Common.Security.Enums;
using FluentValidation;

namespace Security.Application.Features.User.AddPermissionToRole;

public class AddPermissionToRoleRequestValidator : AbstractValidator<AddPermissionToRoleRequest>
{
    public AddPermissionToRoleRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("AddPermissionToRoleRequest can't be null");
        RuleFor(f => f.RoleId).GreaterThan(0).WithMessage("AddPermissionToRoleRequest.RoleId can't be smaller than 1");
        RuleFor(f => f.RoleId).Custom(((i, context) =>
        {
            var role = Roles.FromValue(i);
            if (role != null) return;
            context.AddFailure(context.PropertyPath, "AddPermissionToRoleRequest.RoleId is not recognized");
        }));
        RuleFor(f => f.PermissionId).GreaterThan(0).WithMessage("AddPermissionToRoleRequest.PermissionId can't be smaller than 1");
        RuleFor(f => f.PermissionId).Custom(((i, context) =>
        {
            var role = Permissions.FromValue(i);
            if (role != null) return;
            context.AddFailure(context.PropertyPath, "AddPermissionToRoleRequest.PermissionId is not recognized");
        }));
    }
}