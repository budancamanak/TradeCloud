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

namespace Security.Application.Features.User.AddRoleToUser;

public class AddRoleToUserRequestHandler(
    IValidator<AddRoleToUserRequest> validator,
    IUserService userService,
    IUserRepository repository,
    IMapper mapper,
    ILogger<AddRoleToUserRequestHandler> logger)
    : IRequestHandler<AddRoleToUserRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(AddRoleToUserRequest request, CancellationToken cancellationToken)
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

            logger.LogWarning(UserLogEvents.AddRoleToUser,
                "Adding role[{Role}] to user with Id: {UserId} and username: {Username}", role.Name,
                user.Id, user.Username);

            var mr = await repository.AddRoleToUser(user.Id, role);
            if (mr.IsSuccess) return MethodResponse.Success($"Role[{role.Name}] added to user[{user.Username}].");
            logger.LogCritical(UserLogEvents.AddRoleToUser,
                "Failed to add role[{Role}] to user with Id: {UserId} and username: {Username}. Reason: {Reason}",
                role?.Name, user?.Id, user?.Username, mr.Message);
            return MethodResponse.Success($"Failed to add Role[{role.Name}] to user[{user.Username}].");
        }
        catch (Exception e)
        {
            logger.LogCritical(UserLogEvents.AddRoleToUser,
                "Failed to add role[{Role}] to user with Id: {UserId} and username: {Username}. Reason: {Reason}",
                role?.Name, user?.Id, user?.Username, e.Message);
            return MethodResponse.Error(e.Message);
        }
    }
}