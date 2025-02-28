using AutoMapper;
using Backend.Application.Abstraction.Services;
using Backend.Domain.Entities;
using Common.Core.DTOs.Backend;
using Common.Core.Enums;

namespace Backend.Application.Mappers;

public class PluginOutputMappingProfile : Profile
{
    public PluginOutputMappingProfile()
    {
        CreateMap<PluginOutput, PluginOutputDto>()
            .ForMember(f => f.SignalType,
                opt => opt.MapFrom((src, _, _, _) => src.PluginSignal.GetStringRepresentation()))
            .ForMember(f => f.PluginName,
                opt => opt.MapFrom((_, _, _, context) => context.Items["PluginName"]));
    }
}