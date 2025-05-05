using Ardalis.GuardClauses;
using Backend.Application.Abstraction.Repositories;
using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Models;
using Common.Web.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class AnalysisExecutionRepository(BackendDbContext dbContext, IValidator<AnalysisExecution> validator)
    : IAnalysisExecutionRepository
{
    public async Task<AnalysisExecution> GetByIdAsync(int id)
    {
        Guard.Against.NegativeOrZero(id);
        var item = await dbContext.AnalysisExecutions.FindAsync(id);
        Guard.Against.Null(item, message: $"Failed to find analysis execution with Id[{id}]");
        return item;
    }

    public async Task<List<AnalysisExecution>> GetAllAsync()
    {
        return await dbContext.AnalysisExecutions.ToListAsync();
    }

    public async Task<MethodResponse> AddAsync(AnalysisExecution item)
    {
        Guard.Against.Null(item);
        await validator.ValidateAndThrowAsync(item);
        var existing = await dbContext.AnalysisExecutions.FindAsync(item.Id);
        Guard.Against.NonNull(existing, "Plugin already registered", AlreadySavedException.Creator);
        existing = dbContext.AnalysisExecutions.FirstOrDefault(f =>
            f.PluginIdentifier == item.PluginIdentifier && f.Timeframe == item.Timeframe &&
            f.StartDate == item.StartDate && f.EndDate == item.EndDate &&
            f.ParamSet == item.ParamSet);
        Guard.Against.NonNull(existing, $"Plugin already registered: {existing?.Id}", AlreadySavedException.Creator);
        await dbContext.AnalysisExecutions.AddAsync(item);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to save analysis execution");
        return MethodResponse.Success(item.Id, "Plugin analysis saved");
    }

    public async Task<MethodResponse> UpdateAsync(AnalysisExecution item)
    {
        Guard.Against.Null(item);
        await validator.ValidateAndThrowAsync(item);
        var existing = await dbContext.AnalysisExecutions.FindAsync(item.Id);
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

    public async Task<MethodResponse> DeleteAsync(AnalysisExecution item)
    {
        Guard.Against.Null(item);
        return await DeleteAsync(item.Id);
    }

    public async Task<MethodResponse> DeleteAsync(int id)
    {
        Guard.Against.NegativeOrZero(id);
        var existing = await dbContext.AnalysisExecutions.FindAsync(id);
        Guard.Against.Null(existing);
        dbContext.AnalysisExecutions.Remove(existing);
        var result = await dbContext.SaveChangesAsync();
        if (result > 0) return MethodResponse.Success(result, $"Analysis Execution[{id}] deleted");
        return MethodResponse.Error($"Failed to delete Analysis Execution[{id}]");
    }

    public async Task<List<AnalysisExecution>> GetUserAnalysisExecutions(int userId)
    {
        Guard.Against.NegativeOrZero(userId);
        var items = await dbContext.AnalysisExecutions.Where(f => f.UserId == userId).ToListAsync();
        return items;
    }

    public async Task<List<AnalysisExecution>> GetUserAnalysisExecutions(int userId, PluginStatus status)
    {
        Guard.Against.NegativeOrZero(userId);
        Guard.Against.EnumOutOfRange(status);
        var items = await dbContext.AnalysisExecutions.Where(f => f.UserId == userId && f.Status == status)
            .ToListAsync();
        return items;
    }

    public async Task<MethodResponse> SetAnalysisExecutionProgress(int id, int increment, int total)
    {
        Guard.Against.NegativeOrZero(id);
        var existing = dbContext.AnalysisExecutions.FirstOrDefault(f => f.Id == id);
        Guard.Against.Null(existing);
        if (existing.ProgressTotal == 0)
        {
            existing.ProgressTotal = total * dbContext.PluginExecutions.Count(f => f.AnalysisExecutionId == id);
        }

        if (increment == total)
        {
            existing.ProgressCurrent = existing.ProgressTotal;
        }
        else
        {
            existing.ProgressCurrent += increment;
            if (existing.ProgressCurrent >= existing.ProgressTotal)
                existing.ProgressCurrent = existing.ProgressTotal;
        }

        var result = await dbContext.SaveChangesAsync();
        if (result > 0) return MethodResponse.Success(result, "AnalysisExecutions updated progress");

        return MethodResponse.Error("Failed to update AnalysisExecutions progress");
    }
}