using AutoMapper;
using Common.Core.Models;
using Common.Logging.Events.Security;
using Common.Security.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Security.Application.Abstraction.Repositories;
using Security.Application.Abstraction.Services;

namespace Security.Application.Features.User.AddPermissionToRole;

public class AddPermissionToRoleRequestHandler(
    IValidator<AddPermissionToRoleRequest> validator,
    IUserService userService,
    IRoleRepository repository,
    IMapper mapper,
    ILogger<AddPermissionToRoleRequestHandler> logger) : IRequestHandler<AddPermissionToRoleRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(AddPermissionToRoleRequest request, CancellationToken cancellationToken)
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

            logger.LogWarning(UserLogEvents.AddPermissionToRole, "Adding permission[{Permission}] to role[{Role}]",
                permission.Name, role.Name);

            var mr = await repository.AddPermissionToRole(role, permission);
            if (mr.IsSuccess)
                return MethodResponse.Success($"Permission[{permission.Name}] added to Role[{role.Name}].");
            logger.LogCritical(UserLogEvents.AddPermissionToRole,
                "Failed to add permission[{Permission}] to role[{Role}]. Reason: {Reason}",
                permission?.Name, role?.Name, mr.Message);
            return MethodResponse.Success($"Failed to add Permission[{permission?.Name}] from Role[{role?.Name}].");
        }
        catch (Exception e)
        {
            logger.LogCritical(UserLogEvents.AddPermissionToRole,
                "Failed to add permission[{Permission}] to role[{Role}]. Reason: {Reason}",
                permission?.Name, role?.Name, e.Message);
            return MethodResponse.Error(e.Message);
        }
    }
}