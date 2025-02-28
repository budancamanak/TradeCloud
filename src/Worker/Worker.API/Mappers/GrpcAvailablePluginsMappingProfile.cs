using AutoMapper;
using Common.Grpc;
using Common.Plugin.Abstraction;
using Google.Protobuf.Collections;

namespace Worker.API.Mappers;

public class GrpcAvailablePluginsMappingProfile : Profile
{
    public GrpcAvailablePluginsMappingProfile()
    {
        CreateMap<RepeatedField<GrpcAvailablePluginInfo>, List<IPlugin.PluginInfo>>()
            .ConvertUsing(src =>
                src.Select(x =>
                    new IPlugin.PluginInfo(x.Name, x.Identifier, x.Version)
                ).ToList());
    }
}