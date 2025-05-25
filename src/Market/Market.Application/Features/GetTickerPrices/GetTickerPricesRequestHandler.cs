using AutoMapper;
using Common.Core.DTOs;
using Market.Application.Abstraction.Repositories;
using Market.Application.Abstraction.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Market.Application.Features.GetTickerPrices;

public class GetTickerPricesRequestHandler(
    IPriceService priceService,
    IPriceRepository priceRepository,
    ITickerService tickerService,
    IMapper mapper,
    ILogger<GetTickerPricesRequestHandler> logger) : IRequestHandler<GetTickerPricesRequest, List<PriceDto>>
{
    public async Task<List<PriceDto>> Handle(GetTickerPricesRequest request, CancellationToken cancellationToken)
    {
        var prices = await priceRepository.GetTickerPricesAsync(request.TickerId, request.Timeframe, request.StartDate,
            request.EndDate);
        return mapper.Map<List<PriceDto>>(prices);
    }
}