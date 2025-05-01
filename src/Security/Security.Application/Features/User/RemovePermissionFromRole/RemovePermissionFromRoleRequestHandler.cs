using AutoMapper;
using Common.Core.Models;
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
        var validated = await validator.ValidateAsync(request, cancellationToken);
        if (validated is { IsValid: false })
        {
            return MethodResponse.Error(string.Join(" && ", validated.Errors));
        }

        var permission = Permissions.FromValue(request.PermissionId)!;
        var role = Roles.FromValue(request.RoleId)!;

        logger.LogWarning("Removing permission[{Permission}] to role[{Role}]", permission.Name, role.Name);

        var mr = await repository.RemovePermissionFromRole(role, permission);
        if (mr.IsSuccess) return MethodResponse.Success($"Permission[{permission.Name}] removed to Role[{role.Name}].");
        return MethodResponse.Success($"Failed to remove Permission[{permission.Name}] from Role[{role.Name}].");
    }
}