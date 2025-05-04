using Common.Security.Abstraction;
using Common.Security.Attributes;
using Grpc.AspNetCore.Server;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Security.Interceptors;

public class ServerAuthInterceptor(ILogger<ServerAuthInterceptor> logger, ISecurityGrpcClient securityClient)
    : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        logger.LogDebug("GRPC: Starting receiving call. Type/Method: {Type} / {Method}",
            MethodType.Unary, context.Method);
        var metadata = context.GetHttpContext()?.GetEndpoint()?.Metadata;
        if (metadata == null)
        {
            // pass?
            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error thrown by {context.Method}.");
                throw;
            }
        }

        var roles = metadata.GetOrderedMetadata<HasRoleAttribute>();
        var permissions = metadata.GetOrderedMetadata<HasPermissionAttribute>();
        var token = context.GetHttpContext().Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var anonEnabled = metadata.GetOrderedMetadata<AllowAnonAttribute>().Count > 0;
        if (string.IsNullOrWhiteSpace(token) && anonEnabled)
        {
            // pass?
            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error thrown by {context.Method}.");
                throw;
            }
        }

        var tokenValidation = await securityClient.ValidateToken(token);
        if (!tokenValidation.IsValid)
        {
            context.Status = new Status(StatusCode.Unauthenticated, "User is not validated");
            return null!;
        }

        var isValid = false;

        foreach (var attr in permissions)
        {
            if (!await securityClient.HasPermissionAsync(token, attr.Permissions)) continue;
            isValid = true;
            break;
        }

        foreach (var attr in roles)
        {
            if (!await securityClient.HasRoleAsync(token, attr.Roles)) continue;
            isValid = true;
            break;
        }

        if (!isValid)
        {
            context.Status = new Status(StatusCode.Unauthenticated, "User is not validated");
            return null!;
        }

        Console.WriteLine(">>>");
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error thrown by {context.Method}.");
            throw;
        }
    }
}