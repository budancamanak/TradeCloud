using AutoMapper;
using Backend.Application.Abstraction.Services;
using Backend.Application.Features.Execution.CreatePluginExecution;
using Backend.Application.Features.Execution.RunPluginExecution;
using Backend.Domain.Entities;
using Common.Core.Enums;
using Common.Messaging.Events.PluginExecution;

namespace Backend.Application.Mappers;

public class AnalysisExecutionMappingProfile : Profile
{
    public AnalysisExecutionMappingProfile(ITickerService tickerService, IPluginService pluginService)
    {
        // todo will be in AnalysisExecutionMappingProfile
        CreateMap<CreatePluginExecutionRequest, AnalysisExecution>()
            .ForMember(f => f.TickerId,
                opt => opt.MapFrom((src, _, _, _) =>
                    tickerService.GetTickerWithSymbol(src.Symbol).GetAwaiter().GetResult().Id))
            .ForMember(f => f.UserId, opt => opt.MapFrom((_, _, _, context) => context.Items["CurrentUserId"]));

        CreateMap<RunPluginExecutionRequest, RunPluginRequestedEvent>().ReverseMap();
        CreateMap<AnalysisExecution, RunPluginRequestedEvent>()
            .ForMember(f => f.Timeframe, opt => opt.MapFrom((src, _, _, _) => src.Timeframe.GetStringRepresentation()))
            .ForMember(f => f.Identifier, opt => opt.MapFrom((src, _, _, _) => src.PluginIdentifier))
            .ForMember(f => f.Ticker, opt => opt.MapFrom((src, _, _, _) => src.TickerId))
            .ForMember(f => f.ExecutionId, opt => opt.MapFrom((src, _, _, _) => src.Id))
            .ForMember(f => f.StartDate, opt => opt.MapFrom((src, _, _, _) => src.StartDate))
            .ForMember(f => f.EndDate, opt => opt.MapFrom((src, _, _, _) => src.EndDate));
    }
}