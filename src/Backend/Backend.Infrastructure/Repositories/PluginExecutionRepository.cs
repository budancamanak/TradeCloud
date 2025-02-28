﻿using Ardalis.GuardClauses;
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
    public async Task<PluginExecution> GetByIdAsync(int id)
    {
        Guard.Against.NegativeOrZero(id);
        var item = await dbContext.PluginExecutions.FirstOrDefaultAsync(f => f.Id == id);
        Guard.Against.Null(item);
        return item;
    }

    public async Task<List<PluginExecution>> GetAllAsync()
    {
        return await dbContext.PluginExecutions.ToListAsync();
    }

    public async Task<MethodResponse> AddAsync(PluginExecution item)
    {
        Guard.Against.Null(item);
        await validator.ValidateAndThrowAsync(item);
        var existing = dbContext.PluginExecutions.FirstOrDefault(f =>
            f.PluginIdentifier == item.PluginIdentifier && f.UserId == item.UserId && f.Timeframe == item.Timeframe &&
            f.StartDate == item.StartDate && f.EndDate == item.EndDate &&
            f.Status != PluginStatus.Failure && f.Status != PluginStatus.Success);
        // Guard.Against.NonNull(existing, "Plugin already registered", () => new AlreadySavedException("Plugin already registered"));
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
        var existing = dbContext.PluginExecutions.FirstOrDefault(f => f.Id == item.Id);
        Guard.Against.Null(existing);
        existing.Status = item.Status;
        existing.ParamSet = item.ParamSet;
        existing.TradingParams = item.TradingParams;
        existing.Timeframe = item.Timeframe;
        existing.StartDate = item.StartDate;
        existing.EndDate = item.EndDate;
        existing.Progress = item.Progress;
        var result = await dbContext.SaveChangesAsync();
        if (result > 0) return MethodResponse.Success(result, "Execution updated");
        return MethodResponse.Error("Failed to update execution");
    }

    public Task<MethodResponse> DeleteAsync(PluginExecution item)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<PluginExecution> GetNextWaitingPluginExecution()
    {
        throw new NotImplementedException();
    }

    public async Task<List<PluginExecution>> GetWaitingPluginExecutions()
    {
        var items = await dbContext.PluginExecutions
            // .Where(f => f.Status == PluginStatus.Init || f.Status == PluginStatus.Queued)
            .ToListAsync();
        return items;
    }

    public async Task<List<PluginExecution>> GetActivePluginExecutions()
    {
        var items = await dbContext.PluginExecutions
            .Where(f => f.Status > PluginStatus.Init && f.Status < PluginStatus.Failure).ToListAsync();
        return items;
    }

    public async Task<List<PluginExecution>> GetPluginExecutionsWithStatus(PluginStatus status)
    {
        var items = await dbContext.PluginExecutions
            .Where(f => f.Status == status).ToListAsync();
        return items;
    }

    public async Task<List<PluginExecution>> GetPluginExecutionsForTicker(int tickerId)
    {
        var items = await dbContext.PluginExecutions
            .Where(f => f.TickerId == tickerId).ToListAsync();
        return items;
    }

    public async Task<List<PluginExecution>> GetPluginExecutionsWithIdentifier(string identifier)
    {
        var items = await dbContext.PluginExecutions
            .Where(f => f.PluginIdentifier == identifier).ToListAsync();
        return items;
    }

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

    public async Task<MethodResponse> SetPluginStatus(int id, PluginStatus status)
    {
        Guard.Against.NegativeOrZero(id);
        var existing = dbContext.PluginExecutions.FirstOrDefault(f => f.Id == id);
        Guard.Against.Null(existing);
        if (status > existing.Status)
            existing.Status = status;
        var result = await dbContext.SaveChangesAsync();
        if (result > 0) return MethodResponse.Success(result, "Execution updated status");
        return MethodResponse.Error("Failed to update execution status");
    }
}