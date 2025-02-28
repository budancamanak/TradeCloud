using AutoMapper;
using Backend.Domain.Entities;
using Common.Core.DTOs.Backend;

namespace Backend.Application.Mappers;

public class TrackListMappingProfile : Profile
{
    public TrackListMappingProfile()
    {
        CreateMap<TrackListDto, TrackList>();
        CreateMap<TrackList, TrackListDto>()
            .ForMember(f => f.Symbol,
                opt => opt.MapFrom((_, _, _, context) => context.Items["Symbol"]));
    }
}