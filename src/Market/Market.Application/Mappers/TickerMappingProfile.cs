using AutoMapper;
using Common.Core.DTOs;
using Market.Domain.Entities;

namespace Market.Application.Mappers;

public class TickerMappingProfile : Profile
{
    public TickerMappingProfile()
    {
        CreateMap<Ticker, TickerDto>()
            .ForMember(f => f.ExchangeName, opt => opt.MapFrom((src, _) => src.Exchange.Name))
            .ReverseMap();
        
    }
}