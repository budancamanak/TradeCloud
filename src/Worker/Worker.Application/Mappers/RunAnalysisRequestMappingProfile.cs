using AutoMapper;
using Common.Messaging.Events.AnalysisExecution;
using Common.Messaging.Events.PluginExecution;
using Worker.Application.Features.RunAnalysisRequested;
using Worker.Application.Features.RunPluginRequested;

namespace Worker.Application.Mappers;

public class RunAnalysisRequestMappingProfile : Profile
{
    public RunAnalysisRequestMappingProfile()
    {
        CreateMap<RunAnalysisRequest, RunAnalysisRequestedEvent>().ReverseMap();
    }
}