using AutoMapper;
using Backend.Application.Abstraction.Services;
using Backend.Application.Features.TrackList.AddTickerToTrackList;
using Backend.Application.Features.TrackList.RemoveUserTrackList;
using Backend.Domain.Entities;
using Common.Core.DTOs;
using Common.Core.DTOs.Backend;

namespace Backend.Application.Mappers;

public class TrackListMappingProfile : Profile
{
    public TrackListMappingProfile()
    {
        CreateMap<TrackListDto, TrackList>();
        CreateMap<TrackList, TrackListDto>();
        CreateMap<TrackList, AddTickerToTrackListRequest>().ReverseMap();
        CreateMap<TrackList, RemoveUserTrackListRequest>().ReverseMap();
    }
}