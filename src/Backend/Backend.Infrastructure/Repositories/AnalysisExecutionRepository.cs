using Ardalis.GuardClauses;
using Backend.Application.Abstraction.Repositories;
using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Models;
using Common.Web.Exceptions;
using FluentValidation;

namespace Backend.Infrastructure.Repositories;

public class AnalysisExecutionRepository(BackendDbContext dbContext, IValidator<AnalysisExecution> validator)
    : IAnalysisExecutionRepository
{
    public Task<AnalysisExecution> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<AnalysisExecution>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<MethodResponse> AddAsync(AnalysisExecution item)
    {
        Guard.Against.Null(item);
        await validator.ValidateAndThrowAsync(item);
        var existing = dbContext.AnalysisExecutions.FirstOrDefault(f => f.Id == item.Id);
        Guard.Against.NonNull(existing, "Plugin already registered", AlreadySavedException.Creator);
        existing = dbContext.AnalysisExecutions.FirstOrDefault(f =>
            f.PluginIdentifier == item.PluginIdentifier && f.Timeframe == item.Timeframe &&
            f.StartDate == item.StartDate && f.EndDate == item.EndDate &&
            f.ParamSet == item.ParamSet && f.Progress < 1.0);
        Guard.Against.NonNull(existing, "Plugin already registered", AlreadySavedException.Creator);
        await dbContext.AnalysisExecutions.AddAsync(item);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to save analysis execution");
        return MethodResponse.Success(item.Id, "Plugin analysis saved");
    }

    public async Task<MethodResponse> UpdateAsync(AnalysisExecution item)
    {
        Guard.Against.Null(item);
        await validator.ValidateAndThrowAsync(item);
        var existing = dbContext.AnalysisExecutions.FirstOrDefault(f => f.Id == item.Id);
        Guard.Against.Null(existing);
        existing.ParamSet = item.ParamSet;
        existing.TradingParams = item.TradingParams;
        existing.StartDate = item.StartDate;
        existing.EndDate = item.EndDate;
        existing.Timeframe = item.Timeframe;
        existing.PluginIdentifier = item.PluginIdentifier;
        existing.TickerId = item.TickerId;
        dbContext.Update(existing);
        var result = await dbContext.SaveChangesAsync();
        if (result > 0) return MethodResponse.Success(result, "Execution updated");
        return MethodResponse.Error("Failed to update execution");
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