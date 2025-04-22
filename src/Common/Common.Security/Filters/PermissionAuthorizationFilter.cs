using Common.Security.Abstraction;
using Common.Security.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace Common.Security.Filters;

public class PermissionAuthorizationFilter(
    ISecurityGrpcClient securityClient,
    IHttpContextAccessor contextAccessor)
    : IAsyncAuthorizationFilter
{
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var metadata = endpoint?.Metadata;
        if (metadata == null) return;

        foreach (var attr in metadata.GetOrderedMetadata<HasPermissionAttribute>())
            if (!await securityClient.HasPermissionAsync(token, attr.Permission))
                context.Result = new ForbidResult();

        foreach (var attr in metadata.GetOrderedMetadata<HasRoleAttribute>())
            if (!await securityClient.HasRoleAsync(token, attr.Role))
                context.Result = new ForbidResult();

        foreach (var attr in metadata.GetOrderedMetadata<HasScopeAttribute>())
            if (!await securityClient.HasScopeAsync(token, attr.Scope))
                context.Result = new ForbidResult();

        foreach (var attr in metadata.GetOrderedMetadata<HasPolicyAttribute>())
            if (!await securityClient.HasPolicyAsync(token, attr.Policy))
                context.Result = new ForbidResult();
    }
}