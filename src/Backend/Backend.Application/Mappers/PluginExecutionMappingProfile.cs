using AutoMapper;
using Backend.Application.Abstraction.Services;
using Backend.Domain.Entities;
using Common.Core.DTOs.Backend;
using Common.Core.Enums;

namespace Backend.Application.Mappers;

public class PluginExecutionMappingProfile : Profile
{
    public PluginExecutionMappingProfile( )
    {
        // todo update context.Items to use pluginService & tickerService below !! 
        // todo will be hard to supply context.Items when converting list items.
        // CreateMap<PluginExecution, PluginExecutionsDto>()
        //     .ForMember(f => f.Timeframe,
        //         opt => opt.MapFrom((src, dst, _, _) => src.Timeframe.GetStringRepresentation()))
        //     .ForMember(f => f.PluginName,
        //         opt => opt.MapFrom((src, _, _, _) =>
        //             pluginService.GetPluginInfo(src.PluginIdentifier).GetAwaiter().GetResult().Name))
        //     .ForMember(f => f.Status, opt => opt.MapFrom((src, _, _, _) => src.Status.GetStringRepresentation()))
        //     .ForMember(f => f.Symbol,
        //         opt => opt.MapFrom((src, _, _, _) =>
        //             tickerService.GetTickerWithId(src.TickerId).GetAwaiter().GetResult().Id));

        // CreateMap<PluginExecutionsDto, PluginExecution>()
        //     .ForMember(f => f.Timeframe, opt => opt.MapFrom((src, dst, _, _) => src.Timeframe.TimeFrameFromString()))
        //     .ForMember(f => f.TickerId,
        //         opt => opt.MapFrom((src, _, _, _) =>
        //             tickerService.GetTickerWithSymbol(src.Symbol).GetAwaiter().GetResult().Id))
        //     .ForMember(f => f.Status, opt => opt.MapFrom((src, dst, _, _) => src.Status.ToPluginStatus()));

        // CreateMap<PluginExecution, PluginExecutionsDto>()
        //     .ForMember(f => f.Status, opt => opt.MapFrom((src, _, _, _) => src.Status.GetStringRepresentation()));
        //
        // CreateMap<PluginExecutionsDto, PluginExecution>()
        //     .ForMember(f => f.Status, opt => opt.MapFrom((src, dst, _, _) => src.Status.ToPluginStatus()));
    }
}