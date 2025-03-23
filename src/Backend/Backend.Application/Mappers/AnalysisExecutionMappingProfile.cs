﻿using AutoMapper;
using Backend.Domain.Entities;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Messaging.Events.AnalysisExecution;

namespace Backend.Application.Mappers;

public class AnalysisExecutionMappingProfile : Profile
{
    public AnalysisExecutionMappingProfile()
    {
        // todo will be in AnalysisExecutionMappingProfile
        // CreateMap<CreateAnalysisExecutionRequest, AnalysisExecution>()
        //     .ForMember(f => f.TickerId, opt => opt.MapFrom((_, _, _, context) => context.Items["TickerId"]))
        //     .ForMember(f => f.UserId, opt => opt.MapFrom((_, _, _, context) => context.Items["CurrentUserId"])).ReverseMap();
        // CreateMap<RunPluginExecutionRequest, RunPluginRequestedEvent>().ReverseMap();
        // CreateMap<AnalysisExecution, RunPluginRequestedEvent>()
        //     .ForMember(f => f.Timeframe, opt => opt.MapFrom((src, _, _, _) => src.Timeframe.GetStringRepresentation()))
        //     .ForMember(f => f.Identifier, opt => opt.MapFrom((src, _, _, _) => src.PluginIdentifier))
        //     .ForMember(f => f.Ticker, opt => opt.MapFrom((src, _, _, _) => src.TickerId))
        //     .ForMember(f => f.ExecutionId, opt => opt.MapFrom((src, _, _, _) => src.Id))
        //     .ForMember(f => f.StartDate, opt => opt.MapFrom((src, _, _, _) => src.StartDate))
        //     .ForMember(f => f.EndDate, opt => opt.MapFrom((src, _, _, _) => src.EndDate));
        CreateMap<AnalysisExecution, RunAnalysisRequestedEvent>()
            .ForMember(f => f.PluginInfos, opt => opt.MapFrom((_, _, _, context) => context.Items["PluginInfos"]))
            .ForMember(f => f.Timeframe, opt => opt.MapFrom((src, _, _, _) => src.Timeframe.GetStringRepresentation()))
            .ForMember(f => f.Identifier, opt => opt.MapFrom((src, _, _, _) => src.PluginIdentifier))
            .ForMember(f => f.Ticker, opt => opt.MapFrom((src, _, _, _) => src.TickerId))
            .ForMember(f => f.ExecutionId, opt => opt.MapFrom((src, _, _, _) => src.Id))
            .ForMember(f => f.StartDate,
                opt => opt.MapFrom((src, _, _, _) =>
                    src.StartDate.FitDateToTimeFrame(src.Timeframe.GetMilliseconds(), false)))
            .ForMember(f => f.EndDate,
                opt => opt.MapFrom((src, _, _, _) =>
                    src.EndDate.FitDateToTimeFrame(src.Timeframe.GetMilliseconds(), false)));
    }
}