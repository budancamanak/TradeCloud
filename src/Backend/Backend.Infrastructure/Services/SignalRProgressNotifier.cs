using Backend.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Infrastructure.Services;

public interface IProgressNotifier
{
    Task NotifyProgressAsync(int executionId, double progress);
    Task NotifyStatusChangedAsync(int executionId, string status);
    Task NotifyCompletedAsync(int executionId, bool success, string? error);
}

public class SignalRProgressNotifier : IProgressNotifier
{
    private readonly IHubContext<ProgressHub, IProgressClient> _hubContext;

    public SignalRProgressNotifier(IHubContext<ProgressHub, IProgressClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyProgressAsync(int executionId, double progress)
    {
        return _hubContext.Clients
            .Group($"execution:{executionId}")
            .ProgressUpdated(executionId, progress);
    }

    public Task NotifyStatusChangedAsync(int executionId, string status)
    {
        return _hubContext.Clients
            .Group($"execution:{executionId}")
            .StatusChanged(executionId, status);
    }

    public Task NotifyCompletedAsync(int executionId, bool success, string? error)
    {
        return _hubContext.Clients
            .Group($"execution:{executionId}")
            .ExecutionCompleted(executionId, success, error);
    }
}