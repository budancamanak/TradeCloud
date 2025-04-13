using AutoMapper;
using Common.Core.Models;
using Common.Grpc;
using Common.Plugin.Abstraction;
using Google.Protobuf.Collections;

namespace Worker.API.Mappers;

public class GrpcAvailablePluginsMappingProfile : Profile
{
    public GrpcAvailablePluginsMappingProfile()
    {
        CreateMap<RepeatedField<GrpcAvailablePluginInfo>, List<PluginInfo>>()
            .ConvertUsing(src =>
                src.Select(x =>
                    new PluginInfo(x.Name, x.Identifier, x.Version)
                ).ToList());
    }
}