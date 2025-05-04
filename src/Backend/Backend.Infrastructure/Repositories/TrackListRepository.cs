using Ardalis.GuardClauses;
using Backend.Application.Abstraction.Repositories;
using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Common.Core.Extensions;
using Common.Core.Models;
using Common.Web.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class TrackListRepository(BackendDbContext dbContext, IValidator<TrackList> validator) : ITrackListRepository
{
    public Task<TrackList> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<TrackList>> GetAllAsync()
    {
        return await dbContext.TrackLists.ToListAsync();
    }

    public async Task<MethodResponse> AddAsync(TrackList item)
    {
        Guard.Against.Null(item);
        await validator.ValidateAndThrowAsync(item);
        var existing = await dbContext.TrackLists.FindAsync(item.TickerId, item.UserId);
        // var existing =
        //     await dbContext.TrackLists.FirstOrDefaultAsync(f => f.TickerId == item.TickerId && f.UserId == item.UserId);
        Guard.Against.NonNull(existing, "TrackList already registered", AlreadySavedException.Creator);
        await dbContext.TrackLists.AddAsync(item);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to save TrackList");
        return MethodResponse.Success(result, "TrackList analysis saved");
    }

    public Task<MethodResponse> UpdateAsync(TrackList item)
    {
        throw new NotImplementedException();
    }

    public async Task<MethodResponse> DeleteAsync(TrackList item)
    {
        Guard.Against.Null(item);
        await validator.ValidateAndThrowAsync(item);
        return await DeleteAsync(item.UserId, item.TickerId);
    }

    public async Task<MethodResponse> DeleteAsync(int userId, int tickerId)
    {
        Guard.Against.NegativeOrZero(userId);
        Guard.Against.NegativeOrZero(tickerId);
        var existing = await dbContext.TrackLists.FindAsync(userId, tickerId);
        // var existing = dbContext.TrackLists.FirstOrDefault(f => f.TickerId == item.TickerId && f.UserId == item.UserId);
        Guard.Against.Null(existing);
        dbContext.TrackLists.Remove(existing);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to delete TrackList");
        return MethodResponse.Success(result, "TrackList analysis deleted");
    }

    public Task<MethodResponse> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<TrackList>> GetUserTrackListAsync(int userId)
    {
        return await dbContext.TrackLists.Where(f => f.UserId == userId).ToListAsync();
    }

    public async Task<TrackList?> GetUserTrackListAsync(int userId, int tickerId)
    {
        return await dbContext.TrackLists.FindAsync(userId, tickerId);
    }
}