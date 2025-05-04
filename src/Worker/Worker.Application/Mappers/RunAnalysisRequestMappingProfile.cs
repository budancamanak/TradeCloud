using AutoMapper;
using Common.Messaging.Events.AnalysisExecution;
using Worker.Application.Features.RunAnalysisRequested;

namespace Worker.Application.Mappers;

public class RunAnalysisRequestMappingProfile : Profile
{
    public RunAnalysisRequestMappingProfile()
    {
        CreateMap<RunAnalysisRequest, RunAnalysisRequestedEvent>().ReverseMap();
    }
}