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

public class PluginExecutionRepository(BackendDbContext dbContext, IValidator<PluginExecution> validator)
    : IPluginExecutionRepository
{
    
    // Returns true if DB row was modified (i.e. incoming timestamp was newer)
    public async Task<bool> SetPluginProgressIfNewer(int id, double progress, DateTime incomingUtc)
    {
        var rows = await dbContext.Database.ExecuteSqlInterpolatedAsync($@"
        UPDATE ""PluginExecutions""
        SET ""Progress"" = {progress}, ""LastProgressUpdatedAt"" = {incomingUtc}
        WHERE ""Id"" = {id} AND (""LastProgressUpdatedAt"" IS NULL OR ""LastProgressUpdatedAt"" < {incomingUtc}) AND {progress} > ""Progress""
    ");

        return rows > 0;
    }


    public async Task<PluginExecution> GetByIdAsync(int id)
    {
        Guard.Against.NegativeOrZero(id);
        var item = await dbContext.PluginExecutions.FindAsync(id);
        Guard.Against.Null(item);
        return item;
    }

    public async Task<List<PluginExecution>> GetAllAsync()
    {
        return await dbContext.PluginExecutions.OrderByDescending(f => f.Id).ToListAsync();
    }

    public async Task<MethodResponse> AddAsync(PluginExecution item)
    {
        Guard.Against.Null(item);
        await validator.ValidateAndThrowAsync(item);
        var existing = await dbContext.PluginExecutions.FirstOrDefaultAsync(f =>
            f.AnalysisExecutionId == item.AnalysisExecutionId && f.ParamSet == item.ParamSet &&
            f.Status != PluginStatus.Failure && f.Status != PluginStatus.Success);
        Guard.Against.NonNull(existing, "Plugin already registered", AlreadySavedException.Creator);
        await dbContext.PluginExecutions.AddAsync(item);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to save plugin execution");
        return MethodResponse.Success(item.Id, "Plugin execution saved");
    }

    public async Task<MethodResponse> UpdateAsync(PluginExecution item)
    {
        Guard.Against.Null(item);
        await validator.ValidateAndThrowAsync(item);
        var existing = await dbContext.PluginExecutions.FindAsync(item.Id);
        Guard.Against.Null(existing);
        existing.Status = item.Status;
        existing.ParamSet = item.ParamSet;
        existing.Progress = item.Progress;
        existing.Error = item.Error;
        var result = await dbContext.SaveChangesAsync();
        if (result > 0) return MethodResponse.Success(result, "Execution updated");
        return MethodResponse.Error("Failed to update execution");
    }

    public async Task<MethodResponse> DeleteAsync(PluginExecution item)
    {
        Guard.Against.Null(item);
        await validator.ValidateAndThrowAsync(item);
        return await DeleteAsync(item.Id);
    }

    public async Task<MethodResponse> DeleteAsync(int id)
    {
        Guard.Against.NegativeOrZero(id);
        var existing = await dbContext.PluginExecutions.FindAsync(id);
        Guard.Against.Null(existing);
        dbContext.PluginExecutions.Remove(existing);
        var result = await dbContext.SaveChangesAsync();
        if (result > 0) return MethodResponse.Success(result, "Execution deleted");
        return MethodResponse.Error("Failed to delete execution");
    }

    public Task<PluginExecution> GetNextWaitingPluginExecution()
    {
        throw new NotImplementedException();
    }

    public async Task<List<PluginExecution>> GetWaitingPluginExecutions()
    {
        var items = await dbContext.PluginExecutions
            // .Where(f => f.Status == PluginStatus.Init || f.Status == PluginStatus.Queued)
            .OrderByDescending(f => f.Id)
            .ToListAsync();
        return items;
    }

    public async Task<List<PluginExecution>> GetActivePluginExecutions(int analysisId)
    {
        var items = await dbContext.PluginExecutions
            .Where(f => f.AnalysisExecutionId == analysisId &&
                        f.Status != PluginStatus.Failure &&
                        f.Status != PluginStatus.Success &&
                        f.Status != PluginStatus.Init).OrderByDescending(f => f.Id).ToListAsync();
        return items;
    }

    public async Task<List<PluginExecution>> GetPluginExecutionsWithStatus(PluginStatus status)
    {
        var items = await dbContext.PluginExecutions
            .Where(f => f.Status == status).OrderByDescending(f => f.Id).ToListAsync();
        return items;
    }

    public async Task<List<PluginExecution>> GetPluginExecutionsWithStatus(int analysisId, params PluginStatus[] status)
    {
        var items = await dbContext.PluginExecutions
            .Where(f => f.AnalysisExecutionId == analysisId && status.Contains(f.Status)).OrderByDescending(f => f.Id)
            .ToListAsync();
        return items;
    }

    public async Task<List<PluginExecution>> GetPluginOfAnalysis(int analysisId)
    {
        Guard.Against.NegativeOrZero(analysisId);
        var items = await dbContext.PluginExecutions.Where(f => f.AnalysisExecutionId == analysisId)
            .OrderByDescending(f => f.Id).ToListAsync();
        return items;
    }

    // public async Task<List<PluginExecution>> GetPluginExecutionsForTicker(int tickerId)
    // {
    //     var items = await dbContext.PluginExecutions
    //         .Where(f => f.TickerId == tickerId).ToListAsync();
    //     return items;
    // }
    //
    // public async Task<List<PluginExecution>> GetPluginExecutionsWithIdentifier(string identifier)
    // {
    //     var items = await dbContext.PluginExecutions
    //         .Where(f => f.PluginIdentifier == identifier).ToListAsync();
    //     return items;
    // }

    public async Task<MethodResponse> SetPluginProgress(int id, double progress)
    {
        Guard.Against.NegativeOrZero(id);
        var existing = dbContext.PluginExecutions.FirstOrDefault(f => f.Id == id);
        Guard.Against.Null(existing);
        if (progress > existing.Progress)
        {
            existing.Progress = progress;
            var result = await dbContext.SaveChangesAsync();
            if (result > 0) return MethodResponse.Success(result, "Execution updated progress");
        }

        return MethodResponse.Error("Failed to update execution progress");
    }


    public async Task<MethodResponse> SetPluginError(int id, string error)
    {
        Guard.Against.NegativeOrZero(id);
        var existing = dbContext.PluginExecutions.FirstOrDefault(f => f.Id == id);
        Guard.Against.Null(existing);
        existing.Error = error;
        var result = await dbContext.SaveChangesAsync();
        if (result > 0) return MethodResponse.Success(result, "Execution updated progress");

        return MethodResponse.Error("Failed to update execution progress");
    }

    public async Task<MethodResponse> SetPluginStatus(int id, PluginStatus status)
    {
        Guard.Against.NegativeOrZero(id);
        var existing = dbContext.PluginExecutions.FirstOrDefault(f => f.Id == id);
        Guard.Against.Null(existing, message: $"Failed to find plugin with {id}. Was updating status to :{status}");
        if (status > existing.Status)
            existing.Status = status;
        switch (status)
        {
            case PluginStatus.Queued:
                existing.QueuedDate = DateTime.UtcNow;
                break;
            case PluginStatus.Failure:
            case PluginStatus.Success:
                existing.FinishDate = DateTime.UtcNow;
                // Force progress to 100% on any completion
                existing.Progress = 1.0;
                existing.LastProgressUpdatedAt = DateTime.UtcNow;
                break;
            case PluginStatus.Running:
                existing.RunStartDate = DateTime.UtcNow;
                break;
        }

        var result = await dbContext.SaveChangesAsync();
        if (result > 0) return MethodResponse.Success(result, "Execution updated status");
        return MethodResponse.Error("Failed to update execution status");
    }
}