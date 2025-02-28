using AutoMapper;
using Backend.Application.Abstraction.Services;
using Backend.Domain.Entities;
using Common.Core.Enums;

namespace Backend.Application.Features.Execution.CreatePluginExecution;

public class CreatePluginExecutionRequestMappingProfile
{
    
}

// public class CreatePluginExecutionRequestMappingProfile : Profile
// {
//     public CreatePluginExecutionRequestMappingProfile(ITickerService tickerService)
//     {
//         CreateMap<CreatePluginExecutionRequest, PluginExecution>()
//             .ForMember(f => f.Status, opt => opt.MapFrom((_, _, _, _) => PluginStatus.Init))
//             .ForMember(f => f.TickerId,
//                 opt => opt.MapFrom((_, _, _, context) =>context.Items["TickerId"]
//                     // tickerService.GetTickerWithSymbol(src.Symbol).GetAwaiter().GetResult().Id
//                 ));
//     }
// }