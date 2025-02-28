using Backend.Application.Abstraction.Repositories;
using Backend.Domain.Entities;
using Common.Core.Models;

namespace Backend.Infrastructure.Repositories;

public class TrackListRepository:ITrackListRepository
{
    public Task<TrackList> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<TrackList>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> AddAsync(TrackList item)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> UpdateAsync(TrackList item)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> DeleteAsync(TrackList item)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<TrackList>> GetUserTrackListAsync(int userId)
    {
        throw new NotImplementedException();
    }
}