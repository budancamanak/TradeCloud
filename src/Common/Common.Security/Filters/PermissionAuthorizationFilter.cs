using System.Security.Claims;
using Common.Security.Abstraction;
using Common.Security.Attributes;
using Common.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

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
        var tokenValidation = await securityClient.ValidateToken(token);
        if (!tokenValidation.IsValid)
        {
            context.Result = new ForbidResult();
            return;
        }
        
        foreach (var attr in metadata.GetOrderedMetadata<HasPermissionAttribute>())
        {
            if (await securityClient.HasPermissionAsync(token, attr.Permission.ToString())) continue;
            context.Result = new ForbidResult();
            return;
        }

        foreach (var attr in metadata.GetOrderedMetadata<HasRoleAttribute>())
        {
            if (await securityClient.HasRoleAsync(token, attr.Role)) continue;
            context.Result = new ForbidResult();
            return;
        }

        foreach (var attr in metadata.GetOrderedMetadata<HasScopeAttribute>())
        {
            if (await securityClient.HasScopeAsync(token, attr.Scope)) continue;
            context.Result = new ForbidResult();
            return;
        }

        foreach (var attr in metadata.GetOrderedMetadata<HasPolicyAttribute>())
        {
            if (await securityClient.HasPolicyAsync(token, attr.Policy)) continue;
            context.Result = new ForbidResult();
            return;
        }

        context.HttpContext.Session.SetString("UserId", tokenValidation.UserId);
        // context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{new Claim()}))
    }
}