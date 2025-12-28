using Microsoft.AspNetCore.SignalR;

namespace Backend.Infrastructure.Hubs;

public interface IProgressClient
{
    Task ProgressUpdated(int executionId, double progress);
    Task StatusChanged(int executionId, string status);
    Task ExecutionCompleted(int executionId, bool success, string? error);
}

public class ProgressHub : Hub<IProgressClient>
{
    public async Task SubscribeToExecution(int executionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"execution:{executionId}");
    }

    public async Task UnsubscribeFromExecution(int executionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"execution:{executionId}");
    }
}