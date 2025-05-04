using Ardalis.GuardClauses;
using AutoMapper;
using Common.Core.Models;
using Common.Logging.Events.Security;
using Common.Security.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Security.Application.Abstraction.Repositories;
using Security.Application.Abstraction.Services;

namespace Security.Application.Features.User.RemoveRoleFromUser;

public class RemoveRoleFromUserRequestHandler(
    IValidator<RemoveRoleFromUserRequest> validator,
    IUserService userService,
    IUserRepository repository,
    IMapper mapper,
    ILogger<RemoveRoleFromUserRequestHandler> logger) : IRequestHandler<RemoveRoleFromUserRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(RemoveRoleFromUserRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.User? user = null;
        Roles? role = null;
        try
        {
            var validated = await validator.ValidateAsync(request, cancellationToken);
            if (validated is { IsValid: false })
            {
                return MethodResponse.Error(string.Join(" && ", validated.Errors));
            }

            user = await repository.GetByIdAsync(request.UserId);
            Guard.Against.Null(user);
            role = Roles.FromValue(request.RoleId)!;

            logger.LogWarning(UserLogEvents.RemoveRoleFromUser,
                "Removing role[{Role}] from user with Id: {UserId} and username: {Username}", role.Name,
                user.Id, user.Username);

            var mr = await repository.RemoveRoleFromUser(user.Id, role.Value);
            if (mr.IsSuccess) return MethodResponse.Success($"Role[{role.Name}] removed from user[{user.Username}].");
            logger.LogCritical(UserLogEvents.RemovePermissionFromRole,
                "Failed to remove role[{Role}] from user with Id: {UserId} and username: {Username}. Reason: {Reason}",
                role?.Name, request.UserId, user?.Username, mr.Message);
            return MethodResponse.Success($"Failed to remove Role[{role?.Name}] from user[{user?.Username}].");
        }
        catch (Exception e)
        {
            logger.LogCritical(UserLogEvents.RemovePermissionFromRole,
                "Failed to remove role[{Role}] from user with Id: {UserId} and username: {Username}. Reason: {Reason}",
                role?.Name, request.UserId, user?.Username, e.Message);
            return MethodResponse.Error(e.Message);
        }
    }
}