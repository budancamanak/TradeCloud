using AutoMapper;
using Backend.API.Models;
using Backend.Application.Features.Execution.CreateAnalysisExecution;
using Common.Core.Enums;
using Common.Core.Extensions;

namespace Backend.API.Mappers;

public class AnalysisModelsMappingProfile : Profile
{
    public AnalysisModelsMappingProfile()
    {
        CreateMap<CreateAnalysisExecutionModel, CreateAnalysisExecutionRequest>()
            .ForMember(f => f.Timeframe, opt => opt.MapFrom((src, _, _, _) => src.Timeframe.TimeFrameFromString()))
            .ForMember(f => f.StartDate,
                opt => opt.MapFrom((src, _, _, _) =>
                    src.StartDate.FitDateToTimeFrame(src.Timeframe.TimeFrameFromString().GetMilliseconds(), true)))
            .ForMember(f => f.EndDate,
                opt => opt.MapFrom((src, _, _, _) =>
                    src.EndDate.GetValueOrDefault(DateTime.UtcNow)
                        .FitDateToTimeFrame(src.Timeframe.TimeFrameFromString().GetMilliseconds(), false)));
    }
}