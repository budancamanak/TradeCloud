using Backend.Domain.Entities;
using Common.Application.Repositories;
using Common.Core.Enums;
using Common.Core.Models;

namespace Backend.Application.Abstraction.Repositories;

public interface IPluginExecutionRepository : IAsyncRepository<PluginExecution>
{
    Task<PluginExecution> GetNextWaitingPluginExecution();
    Task<List<PluginExecution>> GetWaitingPluginExecutions();
    Task<List<PluginExecution>> GetActivePluginExecutions();

    Task<List<PluginExecution>> GetPluginExecutionsWithStatus(PluginStatus status);

    // Task<List<PluginExecution>> GetPluginExecutionsForTicker(int tickerId);
    // Task<List<PluginExecution>> GetPluginExecutionsWithIdentifier(string identifier);
    Task<MethodResponse> SetPluginProgress(int id, double progress);
    Task<MethodResponse> SetPluginStatus(int id, PluginStatus status);
}