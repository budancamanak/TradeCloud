using Common.Core.DTOs;
using Common.Core.Enums;
using MediatR;

namespace Market.Application.Features.GetTickerPrices;

public class GetTickerPricesRequest : IRequest<List<PriceDto>>
{
    public int TickerId { get; set; }
    public Timeframe Timeframe { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}