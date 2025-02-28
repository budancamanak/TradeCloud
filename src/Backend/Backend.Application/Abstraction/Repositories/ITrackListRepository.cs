using Backend.Domain.Entities;
using Common.Application.Repositories;

namespace Backend.Application.Abstraction.Repositories;

public interface ITrackListRepository : IAsyncRepository<TrackList>
{
    Task<List<TrackList>> GetUserTrackListAsync(int userId);
}