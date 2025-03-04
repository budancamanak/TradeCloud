using Backend.Application.Abstraction.Repositories;
using Backend.Domain.Entities;
using Common.Core.Models;

namespace Backend.Infrastructure.Repositories;

public class AnalysisExecutionRepository : IAnalysisExecutionRepository
{
    public Task<AnalysisExecution> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<AnalysisExecution>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> AddAsync(AnalysisExecution item)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> UpdateAsync(AnalysisExecution item)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> DeleteAsync(AnalysisExecution item)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}