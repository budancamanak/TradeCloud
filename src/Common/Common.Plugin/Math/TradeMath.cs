using System.Text;
using Common.Application.Repositories;
using Common.Core.DTOs;
using Skender.Stock.Indicators;

namespace Common.Plugin.Math;

public class TradeMath
{
    private readonly ICacheService cache;
    private readonly List<Quote> prices;
    private readonly int analysisExecutionId;

    public TradeMath(ICacheService cache, int analysisExecutionId, List<PriceDto> priceDtos)
    {
        this.analysisExecutionId = analysisExecutionId;
        this.cache = cache;
        this.prices = priceDtos.ToQuotes();
    }

    private TOut CachedFunc<TOut>(string cacheKey, Func<TOut> func)
    {
        var cached = cache.GetAsync<TOut>(cacheKey).GetAwaiter().GetResult();
        if (cached != null) return cached;
        cached = func();
        cache.SetAsync(cacheKey, cached, TimeSpan.FromMinutes(30)).GetAwaiter().GetResult();
        return cached;
    }

    private string GenerateCacheKey(string prefix, params object[]? args)
    {
        StringBuilder sb = new StringBuilder(prefix);
        if (args != null)
        {
            sb.Append("_");
            foreach (var value in args)
            {
                sb.Append(value).Append("_");
            }
        }

        return sb.ToString();
    }

    public IEnumerable<SmaResult> GetSma(int lookbackPeriods)
    {
        var key = GenerateCacheKey("GET_SMA", analysisExecutionId, lookbackPeriods);
        return CachedFunc(key, () => prices.GetSma(lookbackPeriods));
    }
    public IEnumerable<AdlResult> GetAdl(int smaPeriods)
    {
        var key = GenerateCacheKey("GET_SMA", analysisExecutionId, smaPeriods);
        return CachedFunc(key, () => prices.GetAdl(smaPeriods));
    }
}