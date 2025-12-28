using Common.Security.Abstraction;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Infrastructure.Hubs;

public class SignalRAuthorizationFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var httpContext = invocationContext.Context.GetHttpContext();
        if (httpContext?.Items["CurrentUserId"] == null)
        {
            throw new HubException("Unauthorized");
        }

        return await next(invocationContext);
    }

    public async Task OnConnectedAsync(
        HubLifetimeContext context,
        Func<HubLifetimeContext, Task> next)
    {
        var httpContext = context.Context.GetHttpContext();
        var securityClient = httpContext?.RequestServices.GetService<ISecurityGrpcClient>();

        var token = httpContext?.Request.Query["access_token"].ToString();
        if (string.IsNullOrEmpty(token))
        {
            token = httpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        }

        if (string.IsNullOrWhiteSpace(token) || securityClient == null)
        {
            throw new HubException("Unauthorized");
        }

        var validation = await securityClient.ValidateToken(token);
        if (!validation.IsValid)
        {
            throw new HubException("Unauthorized");
        }

        httpContext!.Items["CurrentUser"] = validation.UserId;
        httpContext.Items["CurrentUserId"] = validation.Id;

        await next(context);
    }
}