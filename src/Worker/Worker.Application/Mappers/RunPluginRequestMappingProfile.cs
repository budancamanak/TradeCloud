using AutoMapper;
using Common.Messaging.Events.PluginExecution;
using Worker.Application.Features.RunPluginRequested;

namespace Worker.Application.Mappers;

public class RunPluginRequestMappingProfile : Profile
{
    public RunPluginRequestMappingProfile()
    {
        CreateMap<RunPluginRequest, RunPluginRequestedEvent>().ReverseMap();
    }
}