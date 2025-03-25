using Backend.Domain.Entities;
using Common.Application.Repositories;

namespace Backend.Application.Abstraction.Repositories;

public interface IAnalysisExecutionRepository : IAsyncRepository<AnalysisExecution>
{
}