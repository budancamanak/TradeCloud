using AutoMapper;
using Common.Core.DTOs;
using Market.Domain.Entities;

namespace Market.Application.Mappers;

public class PriceMappingProfile : Profile
{
    public PriceMappingProfile()
    {
        CreateMap<Price, PriceDto>();
        CreateMap<PriceDto, Price>()
            .ForMember(f => f.TickerId,
                opt => opt.MapFrom((_, _, _, context) => context.Items["TickerId"]))
            .ForMember(f => f.Timeframe,
                opt => opt.MapFrom((_, _, _, context) => context.Items["TimeFrame"]));
    }
}