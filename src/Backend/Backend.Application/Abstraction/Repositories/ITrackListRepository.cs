﻿using Backend.Domain.Entities;
using Common.Application.Repositories;
using Common.Core.Models;

namespace Backend.Application.Abstraction.Repositories;

public interface ITrackListRepository : IAsyncRepository<TrackList>
{
    Task<List<TrackList>> GetUserTrackListAsync(int userId);
    Task<TrackList?> GetUserTrackListAsync(int userId,int tickerId);
    Task<MethodResponse> DeleteAsync(int userId,int tickerId);
}