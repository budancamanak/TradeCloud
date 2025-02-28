using AutoMapper;
using Common.Core.DTOs;
using Market.Domain.Entities;

namespace Market.Application.Mappers;

public class ExchangeMappingProfile : Profile
{
    public ExchangeMappingProfile()
    {
        CreateMap<Exchange, ExchangeDto>().ReverseMap();
    }
}