using Backend.Domain.Entities;
using Common.Application.Repositories;
using Common.Core.Enums;

namespace Backend.Application.Abstraction.Repositories;

public interface IAnalysisExecutionRepository : IAsyncRepository<AnalysisExecution>
{
    Task<List<AnalysisExecution>> GetUserAnalysisExecutions(int userId);
    Task<List<AnalysisExecution>> GetUserAnalysisExecutions(int userId, PluginStatus status);
}