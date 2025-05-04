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

public class PluginOutputRepository(BackendDbContext dbContext, IValidator<PluginOutput> validator)
    : IPluginOutputRepository
{
    public async Task<PluginOutput> GetByIdAsync(int id)
    {
        Guard.Against.NegativeOrZero(id);
        var item = await dbContext.PluginOutputs.FindAsync(id);
        Guard.Against.Null(item);
        return item;
    }

    public async Task<List<PluginOutput>> GetAllAsync()
    {
        return await dbContext.PluginOutputs.ToListAsync();
    }

    public async Task<MethodResponse> AddAsync(PluginOutput item)
    {
        Guard.Against.Null(item);
        await validator.ValidateAndThrowAsync(item);
        var existing = dbContext.PluginOutputs.FirstOrDefault(f =>
            f.SignalDate == item.SignalDate && f.PluginId == item.PluginId && f.PluginSignal == item.PluginSignal);
        Guard.Against.NonNull(existing, "Plugin output registered", AlreadySavedException.Creator);
        await dbContext.PluginOutputs.AddAsync(item);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to save plugin output");
        return MethodResponse.Success(item.Id, "Plugin output saved");
    }

    public Task<MethodResponse> UpdateAsync(PluginOutput item)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> DeleteAsync(PluginOutput item)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<PluginOutput>> GetPluginOutputs(int pluginId)
    {
        Guard.Against.NegativeOrZero(pluginId);
        return await dbContext.PluginOutputs.Where(f => f.PluginId == pluginId).ToListAsync();
    }

    public async Task<List<PluginOutput>> GetPluginOutputsOfSignalType(int pluginId, SignalType signalType)
    {
        Guard.Against.NegativeOrZero(pluginId);
        Guard.Against.EnumOutOfRange(signalType);
        return await dbContext.PluginOutputs.Where(f => f.PluginId == pluginId && f.PluginSignal == signalType)
            .ToListAsync();
    }
}