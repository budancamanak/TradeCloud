using AutoMapper;
using Backend.Application.Abstraction.Services;
using Backend.Domain.Entities;
using Common.Core.Enums;

namespace Backend.Application.Features.Execution.CreatePluginExecution;

public class CreatePluginExecutionRequestMappingProfile : Profile
{
    public CreatePluginExecutionRequestMappingProfile( )
    {
        // CreateMap<CreatePluginExecutionRequest, AnalysisExecution>()
        //     .ForMember(f => f.TickerId,
        //         opt => opt.MapFrom((src, _, _, _) =>
        //             tickerService.GetTickerWithSymbol(src.Symbol).GetAwaiter().GetResult().Id))
        //     .ForMember(f => f.Status, opt => opt.MapFrom((src, dst, _, context) => PluginStatus.Init))
        //     .ForMember(f => f.UserId, opt => opt.MapFrom((_, _, _, context) => context.Items["CurrentUserId"]));
    }
}