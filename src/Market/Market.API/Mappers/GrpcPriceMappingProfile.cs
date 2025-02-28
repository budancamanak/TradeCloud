using AutoMapper;
using Common.Core.DTOs;
using Common.Core.Enums;
using Common.Grpc;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Market.Application.Features.GetPricesForPlugin.Request;

namespace Market.API.Mappers;

public class GrpcPriceMappingProfile : Profile
{
    public GrpcPriceMappingProfile()
    {
        CreateMap<PriceDto, GrpcPrice>()
            .ForMember(f => f.Timestamp, opt => opt.MapFrom((src, _) => Timestamp.FromDateTime(src.Timestamp)))
            .ForMember(f => f.Close, opt => opt.MapFrom((src, _) => src.Close))
            .ForMember(f => f.High, opt => opt.MapFrom((src, _) => src.High))
            .ForMember(f => f.Low, opt => opt.MapFrom((src, _) => src.Low))
            .ForMember(f => f.Open, opt => opt.MapFrom((src, _) => src.Open));


        CreateMap<GrpcGetPricesRequest, GetPricesForPluginQuery>()
            .ForMember(f => f.PluginId, opt => opt.MapFrom((src, _) => src.PluginId))
            .ForMember(f => f.TickerId, opt => opt.MapFrom((src, _) => src.Ticker))
            .ForMember(f => f.StartDate, opt => opt.MapFrom((src, _) => src.StartDate.ToDateTime()))
            .ForMember(f => f.EndDate, opt => opt.MapFrom((src, _) => src.EndDate.ToDateTime()))
            .ForMember(f => f.Timeframe, opt => opt.MapFrom((src, _) => src.Timeframe.TimeFrameFromString()));

        CreateMap<List<PriceDto>, GrpcGetPricesResponse>()
            .ForMember(dest => dest.Prices, opt => opt.MapFrom(src => ConvertToRepeatedField(src)));

        CreateMap<RepeatedField<GrpcPrice>, List<PriceDto>>()
            .ConvertUsing(src =>
                src.Select(x =>
                    new PriceDto(x.Timestamp.ToDateTime(), x.Open, x.High, x.Close, x.Low,x.Volume)
                ).ToList());
    }

    private RepeatedField<GrpcPrice> ConvertToRepeatedField(List<PriceDto>? prices)
    {
        var repeatedField = new RepeatedField<GrpcPrice>();
        if (prices == null) return repeatedField;
        foreach (var s in prices)
        {
            repeatedField.Add(new GrpcPrice
            {
                Close = s.Close,
                High = s.High,
                Low = s.Low,
                Open = s.Open,
                Timestamp = s.Timestamp.ToTimestamp()
            });
        }

        return repeatedField;
    }
}