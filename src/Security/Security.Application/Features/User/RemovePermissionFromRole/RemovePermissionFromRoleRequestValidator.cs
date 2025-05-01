using Common.Security.Enums;
using FluentValidation;

namespace Security.Application.Features.User.RemovePermissionFromRole;

public class RemovePermissionFromRoleRequestValidator: AbstractValidator<RemovePermissionFromRoleRequest>
{
    public RemovePermissionFromRoleRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("RemovePermissionFromRoleRequest can't be null");
        RuleFor(f => f.RoleId).GreaterThan(0).WithMessage("RemovePermissionFromRoleRequest.RoleId can't be smaller than 1");
        RuleFor(f => f.RoleId).Custom(((i, context) =>
        {
            var role = Roles.FromValue(i);
            if (role != null) return;
            context.AddFailure(context.PropertyPath, "RemovePermissionFromRoleRequest.RoleId is not recognized");
        }));
        RuleFor(f => f.PermissionId).GreaterThan(0).WithMessage("RemovePermissionFromRoleRequest.PermissionId can't be smaller than 1");
        RuleFor(f => f.PermissionId).Custom(((i, context) =>
        {
            var role = Permissions.FromValue(i);
            if (role != null) return;
            context.AddFailure(context.PropertyPath, "RemovePermissionFromRoleRequest.PermissionId is not recognized");
        }));
    }
}