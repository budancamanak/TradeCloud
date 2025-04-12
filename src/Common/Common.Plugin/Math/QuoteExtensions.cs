using Common.Core.DTOs;
using Skender.Stock.Indicators;

namespace Common.Plugin.Math;

public static class QuoteExtensions
{
    public static List<Quote> ToQuotes(this List<PriceDto> prices)
    {
        return prices.Select(dto => dto.ToQuote()).ToList();
    }

    public static Quote ToQuote(this PriceDto price)
    {
        return new Quote
        {
            Close = price.Close,
            Date = price.Timestamp,
            High = price.High,
            Low = price.Low,
            Open = price.Open,
            Volume = price.Volume
        };
    }
}