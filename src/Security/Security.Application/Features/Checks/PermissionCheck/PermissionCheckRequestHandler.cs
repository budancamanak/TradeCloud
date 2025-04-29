using Common.Core.Models;
using Common.Grpc;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Security.Application.Abstraction.Services;

namespace Security.Application.Features.Checks.PermissionCheck;

public class PermissionCheckRequestHandler(
    IUserService userService,
    ILogger<PermissionCheckRequestHandler> logger,
    IValidator<PermissionCheckRequest> validator,
    ITokenService tokenService) : IRequestHandler<PermissionCheckRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(PermissionCheckRequest request, CancellationToken cancellationToken)
    {
        var validated = await validator.ValidateAsync(request, cancellationToken);
        if (validated is { IsValid: false })
        {
            return MethodResponse.Error(string.Join(" && ", validated.Errors));
        }

        logger.LogWarning("Controlling token for role check. IP: {ClientIp}", request.ClientIp);

        var valid = await tokenService.ValidateToken(request.Token, request.ClientIp);
        if (!valid.IsValid) return MethodResponse.Error("Token is invalid");
        var userPermissions = await userService.GetUserPermissions(valid.UserId);
        var hasPermission = userPermissions.FirstOrDefault(f => f.Name == request.Permission) != null;
        return new MethodResponse { IsSuccess = hasPermission };
    }
}