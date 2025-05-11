using Common.Security.Abstraction;
using Common.Security.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.Security.Filters;

public class PermissionAuthorizationFilter(
    ISecurityGrpcClient securityClient,
    IHttpContextAccessor contextAccessor)
    : IAsyncAuthorizationFilter
{
    // private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        var metadata = endpoint?.Metadata;
        if (metadata == null) return;
        var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var anonEnabled = metadata.GetOrderedMetadata<AllowAnonAttribute>().Count > 0;

        if (string.IsNullOrWhiteSpace(token) && anonEnabled)
        {
            context.HttpContext.Items.Add("CurrentUser", "-1");
            return;
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("Token is null");
            context.Result = new ForbidResult();
            return;
        }

        var tokenValidation = await securityClient.ValidateToken(token);
        if (!tokenValidation.IsValid)
        {
            context.Result = new ForbidResult();
            return;
        }

        var isValid = false;

        foreach (var attr in metadata.GetOrderedMetadata<HasPermissionAttribute>())
        {
            if (!await securityClient.HasPermissionAsync(token, attr.Permissions)) continue;
            isValid = true;
            break;
        }

        foreach (var attr in metadata.GetOrderedMetadata<HasRoleAttribute>())
        {
            if (!await securityClient.HasRoleAsync(token, attr.Roles)) continue;
            isValid = true;
            break;
        }

        if (!isValid)
        {
            context.Result = new ForbidResult();
            return;
        }

        // todo will be implemented & enabled later
        // foreach (var attr in metadata.GetOrderedMetadata<HasScopeAttribute>())
        // {
        //     if (await securityClient.HasScopeAsync(token, attr.Scope)) continue;
        //     context.Result = new ForbidResult();
        //     return;
        // }
        //
        // foreach (var attr in metadata.GetOrderedMetadata<HasPolicyAttribute>())
        // {
        //     if (await securityClient.HasPolicyAsync(token, attr.Policy)) continue;
        //     context.Result = new ForbidResult();
        //     return;
        // }

        context.HttpContext.Items.Add("CurrentUser", tokenValidation.UserId);
    }
}