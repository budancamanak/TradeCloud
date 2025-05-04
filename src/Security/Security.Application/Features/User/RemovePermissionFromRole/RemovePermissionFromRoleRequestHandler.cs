using AutoMapper;
using Common.Core.Models;
using Common.Logging.Events.Security;
using Common.Security.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Security.Application.Abstraction.Repositories;
using Security.Application.Abstraction.Services;

namespace Security.Application.Features.User.RemovePermissionFromRole;

public class RemovePermissionFromRoleRequestHandler(
    IValidator<RemovePermissionFromRoleRequest> validator,
    IUserService userService,
    IRoleRepository repository,
    IMapper mapper,
    ILogger<RemovePermissionFromRoleRequestHandler> logger)
    : IRequestHandler<RemovePermissionFromRoleRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(RemovePermissionFromRoleRequest request,
        CancellationToken cancellationToken)
    {
        Permissions? permission = null;
        Roles? role = null;
        try
        {
            var validated = await validator.ValidateAsync(request, cancellationToken);
            if (validated is { IsValid: false })
            {
                return MethodResponse.Error(string.Join(" && ", validated.Errors));
            }

            permission = Permissions.FromValue(request.PermissionId)!;
            role = Roles.FromValue(request.RoleId)!;
            logger.LogWarning(UserLogEvents.RemovePermissionFromRole,
                "Removing permission[{Permission}] to role[{Role}]", permission.Name, role.Name);

            var mr = await repository.RemovePermissionFromRole(role, permission);
            if (mr.IsSuccess)
                return MethodResponse.Success($"Permission[{permission.Name}] removed to Role[{role.Name}].");
            logger.LogCritical(UserLogEvents.RemovePermissionFromRole,
                "Failed to remove permission[{Permission}] from role[{Role}]. Reason: {Reason}",
                permission?.Name, role?.Name, mr.Message);
            return MethodResponse.Success($"Failed to remove Permission[{permission?.Name}] from Role[{role?.Name}].");
        }
        catch (Exception e)
        {
            logger.LogCritical(UserLogEvents.RemovePermissionFromRole,
                "Failed to remove permission[{Permission}] from role[{Role}]. Reason: {Reason}",
                permission?.Name, role?.Name, e.Message);
            return MethodResponse.Error(e.Message);
        }
    }
}