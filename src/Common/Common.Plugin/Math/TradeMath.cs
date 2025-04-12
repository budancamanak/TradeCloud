using System.Runtime.CompilerServices;
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

    private string GenerateCacheKey(object[]? args = null, [CallerMemberName] string callerName = "_caller_method_")
    {
        StringBuilder sb = new StringBuilder(callerName);
        sb.Append("_").Append(analysisExecutionId);
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

    protected static object[] Args(params object[] args)
    {
        return args;
    }

    public IEnumerable<SmaResult> GetSma(int lookbackPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetSma(lookbackPeriods));
    }

    public IEnumerable<AdlResult> GetAdl(int? smaPeriods = null)
    {
        var key = GenerateCacheKey(Args(smaPeriods));
        return CachedFunc(key, () => prices.GetAdl(smaPeriods));
    }

    public IEnumerable<AdxResult> GetAdx(int lookbackPeriods = 14)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetAdx(lookbackPeriods));
    }

    public IEnumerable<AlligatorResult> GetAlligator(int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
    {
        var key = GenerateCacheKey(Args(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset));
        return CachedFunc(key,
            () => prices.GetAlligator(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset));
    }

    public IEnumerable<AlmaResult> GetAlma(int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6.0)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, offset, sigma));
        return CachedFunc(key, () => prices.GetAlma(lookbackPeriods, offset, sigma));
    }

    public IEnumerable<AroonResult> GetAroon(int lookbackPeriods = 25)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetAroon(lookbackPeriods));
    }

    public IEnumerable<AtrResult> GetAtr(int lookbackPeriods = 14)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetAtr(lookbackPeriods));
    }

    public IEnumerable<AtrStopResult> GetAtrStop(int lookbackPeriods = 14,
        double multiplier = 3.0,
        EndType endType = EndType.Close)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, multiplier, endType));
        return CachedFunc(key, () => prices.GetAtrStop(lookbackPeriods, multiplier, endType));
    }

    public IEnumerable<AwesomeResult> GetAwesome(int fastPeriods = 5,
        int slowPeriods = 34)
    {
        var key = GenerateCacheKey(Args(fastPeriods, slowPeriods));
        return CachedFunc(key, () => prices.GetAwesome(fastPeriods, slowPeriods));
    }

    public IEnumerable<BollingerBandsResult> GetBollingerBands(int lookbackPeriods = 20,
        double standardDeviations = 2.0)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, standardDeviations));
        return CachedFunc(key, () => prices.GetBollingerBands(lookbackPeriods, standardDeviations));
    }

    public IEnumerable<BopResult> GetBop(int smoothPeriods = 14)
    {
        var key = GenerateCacheKey(Args(smoothPeriods));
        return CachedFunc(key, () => prices.GetBop(smoothPeriods));
    }

    public IEnumerable<CciResult> GetCci(int lookbackPeriods = 20)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetCci(lookbackPeriods));
    }

    public IEnumerable<ChaikinOscResult> GetChaikinOsc(int fastPeriods = 3,
        int slowPeriods = 10)
    {
        var key = GenerateCacheKey(Args(fastPeriods, slowPeriods));
        return CachedFunc(key, () => prices.GetChaikinOsc(fastPeriods, slowPeriods));
    }

    public IEnumerable<ChandelierResult> GetChandelier(int lookbackPeriods = 22,
        double multiplier = 3.0,
        ChandelierType type = ChandelierType.Long)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, multiplier, type));
        return CachedFunc(key, () => prices.GetChandelier(lookbackPeriods, multiplier, type));
    }

    public IEnumerable<ChopResult> GetChop(int lookbackPeriods = 14)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetChop(lookbackPeriods));
    }

    public IEnumerable<CmfResult> GetCmf(int lookbackPeriods = 20)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetCmf(lookbackPeriods));
    }

    public IEnumerable<CmoResult> GetCmo(int lookbackPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetCmo(lookbackPeriods));
    }

    public IEnumerable<ConnorsRsiResult> GetConnorsRsi(int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100)
    {
        var key = GenerateCacheKey(Args(rsiPeriods, streakPeriods, rankPeriods));
        return CachedFunc(key, () => prices.GetConnorsRsi(rsiPeriods, streakPeriods, rankPeriods));
    }

    public IEnumerable<DemaResult> GetDema(int lookbackPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetDema(lookbackPeriods));
    }

    public IEnumerable<CandleResult> GetDoji(double maxPriceChangePercent = 0.1)
    {
        var key = GenerateCacheKey(Args(maxPriceChangePercent));
        return CachedFunc(key, () => prices.GetDoji(maxPriceChangePercent));
    }

    public IEnumerable<DonchianResult> GetDonchian(int lookbackPeriods = 20)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetDonchian(lookbackPeriods));
    }

    public IEnumerable<DpoResult> GetDpo(int lookbackPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetDpo(lookbackPeriods));
    }

    public IEnumerable<DynamicResult> GetDynamic(int lookbackPeriods,
        double kFactor = 0.6)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, kFactor));
        return CachedFunc(key, () => prices.GetDynamic(lookbackPeriods, kFactor));
    }

    public IEnumerable<ElderRayResult> GetElderRay(int lookbackPeriods = 13)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetElderRay(lookbackPeriods));
    }

    public IEnumerable<EmaResult> GetEma(int lookbackPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetEma(lookbackPeriods));
    }

    public IEnumerable<EpmaResult> GetEpma(int lookbackPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetEpma(lookbackPeriods));
    }

    public IEnumerable<FcbResult> GetFcb(int windowSpan = 2)
    {
        var key = GenerateCacheKey(Args(windowSpan));
        return CachedFunc(key, () => prices.GetFcb(windowSpan));
    }

    public IEnumerable<FisherTransformResult> GetFisherTransform(int lookbackPeriods = 10)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetFisherTransform(lookbackPeriods));
    }

    public IEnumerable<ForceIndexResult> GetForceIndex(int lookbackPeriods = 2)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetForceIndex(lookbackPeriods));
    }

    public IEnumerable<FractalResult> GetFractal(int windowSpan = 2,
        EndType endType = EndType.HighLow)
    {
        var key = GenerateCacheKey(Args(windowSpan, endType));
        return CachedFunc(key, () => prices.GetFractal(windowSpan, endType));
    }

    public IEnumerable<GatorResult> GetGator()
    {
        var key = GenerateCacheKey(Args());
        return CachedFunc(key, () => prices.GetGator());
    }

    public IEnumerable<HeikinAshiResult> GetHeikinAshi()
    {
        var key = GenerateCacheKey(Args());
        return CachedFunc(key, () => prices.GetHeikinAshi());
    }

    public IEnumerable<HmaResult> GetHma(int lookbackPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetHma(lookbackPeriods));
    }

    public IEnumerable<HtlResult> GetHtTrendline()
    {
        var key = GenerateCacheKey(Args());
        return CachedFunc(key, () => prices.GetHtTrendline());
    }

    public IEnumerable<HurstResult> GetHurst(int lookbackPeriods = 100)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetHurst(lookbackPeriods));
    }

    public IEnumerable<IchimokuResult> GetIchimoku(int tenkanPeriods = 9,
        int kijunPeriods = 26,
        int senkouBPeriods = 52)
    {
        var key = GenerateCacheKey(Args(tenkanPeriods, kijunPeriods, senkouBPeriods));
        return CachedFunc(key, () => prices.GetIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods));
    }

    public IEnumerable<KamaResult> GetKama(int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30)
    {
        var key = GenerateCacheKey(Args(erPeriods, fastPeriods, slowPeriods));
        return CachedFunc(key, () => prices.GetKama(erPeriods, fastPeriods, slowPeriods));
    }

    public IEnumerable<KeltnerResult> GetKeltner(int emaPeriods = 20,
        double multiplier = 2.0,
        int atrPeriods = 10)
    {
        var key = GenerateCacheKey(Args(emaPeriods, multiplier, atrPeriods));
        return CachedFunc(key, () => prices.GetKeltner(emaPeriods, multiplier, atrPeriods));
    }

    public IEnumerable<KvoResult> GetKvo(int fastPeriods = 34,
        int slowPeriods = 55,
        int signalPeriods = 13)
    {
        var key = GenerateCacheKey(Args(fastPeriods, slowPeriods, signalPeriods));
        return CachedFunc(key, () => prices.GetKvo(fastPeriods, slowPeriods, signalPeriods));
    }

    public IEnumerable<MacdResult> GetMacd(int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
    {
        var key = GenerateCacheKey(Args(fastPeriods, slowPeriods, signalPeriods));
        return CachedFunc(key, () => prices.GetMacd(fastPeriods, slowPeriods, signalPeriods));
    }

    public IEnumerable<MaEnvelopeResult> GetMaEnvelopes(int lookbackPeriods,
        double percentOffset = 2.5,
        MaType movingAverageType = MaType.SMA)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, percentOffset, movingAverageType));
        return CachedFunc(key, () => prices.GetMaEnvelopes(lookbackPeriods, percentOffset, movingAverageType));
    }

    public IEnumerable<MamaResult> GetMama(double fastLimit = 0.5,
        double slowLimit = 0.05)
    {
        var key = GenerateCacheKey(Args(fastLimit, slowLimit));
        return CachedFunc(key, () => prices.GetMama(fastLimit, slowLimit));
    }

    public IEnumerable<CandleResult> GetMarubozu(double minBodyPercent = 95.0)
    {
        var key = GenerateCacheKey(Args(minBodyPercent));
        return CachedFunc(key, () => prices.GetMarubozu(minBodyPercent));
    }

    public IEnumerable<MfiResult> GetMfi(int lookbackPeriods = 14)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetMfi(lookbackPeriods));
    }

    public IEnumerable<ObvResult> GetObv(int? smaPeriods = null)
    {
        var key = GenerateCacheKey(Args(smaPeriods));
        return CachedFunc(key, () => prices.GetObv(smaPeriods));
    }

    public IEnumerable<ParabolicSarResult> GetParabolicSar(double accelerationStep = 0.02,
        double maxAccelerationFactor = 0.2)
    {
        var key = GenerateCacheKey(Args(accelerationStep, maxAccelerationFactor));
        return CachedFunc(key, () => prices.GetParabolicSar(accelerationStep, maxAccelerationFactor));
    }

    public IEnumerable<PivotPointsResult> GetPivotPoints(PeriodSize windowSize,
        PivotPointType pointType = PivotPointType.Standard)
    {
        var key = GenerateCacheKey(Args(windowSize, pointType));
        return CachedFunc(key, () => prices.GetPivotPoints(windowSize, pointType));
    }

    public IEnumerable<PivotsResult> GetPivots(int leftSpan = 2,
        int rightSpan = 2,
        int maxTrendPeriods = 20,
        EndType endType = EndType.HighLow)
    {
        var key = GenerateCacheKey(Args(leftSpan, rightSpan, maxTrendPeriods, endType));
        return CachedFunc(key, () => prices.GetPivots(leftSpan, rightSpan, maxTrendPeriods, endType));
    }

    public IEnumerable<PmoResult> GetPmo(int timePeriods = 35,
        int smoothPeriods = 20,
        int signalPeriods = 10)
    {
        var key = GenerateCacheKey(Args(timePeriods, smoothPeriods, signalPeriods));
        return CachedFunc(key, () => prices.GetPmo(timePeriods, smoothPeriods, signalPeriods));
    }

    public IEnumerable<PvoResult> GetPvo(int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
    {
        var key = GenerateCacheKey(Args(fastPeriods, slowPeriods, signalPeriods));
        return CachedFunc(key, () => prices.GetPvo(fastPeriods, slowPeriods, signalPeriods));
    }

    public IEnumerable<RenkoResult> GetRenko(Decimal brickSize,
        EndType endType = EndType.Close)
    {
        var key = GenerateCacheKey(Args(brickSize, endType));
        return CachedFunc(key, () => prices.GetRenko(brickSize, endType));
    }

    public IEnumerable<RenkoResult> GetRenkoAtr(int atrPeriods,
        EndType endType = EndType.Close)
    {
        var key = GenerateCacheKey(Args(atrPeriods, endType));
        return CachedFunc(key, () => prices.GetRenkoAtr(atrPeriods, endType));
    }

    public IEnumerable<RocResult> GetRoc(int lookbackPeriods,
        int? smaPeriods = null)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, smaPeriods));
        return CachedFunc(key, () => prices.GetRoc(lookbackPeriods, smaPeriods));
    }

    public IEnumerable<RocWbResult> GetRocWb(int lookbackPeriods,
        int emaPeriods,
        int stdDevPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, emaPeriods, stdDevPeriods));
        return CachedFunc(key, () => prices.GetRocWb(lookbackPeriods, emaPeriods, stdDevPeriods));
    }

    public IEnumerable<RollingPivotsResult> GetRollingPivots(int windowPeriods,
        int offsetPeriods,
        PivotPointType pointType = PivotPointType.Standard)
    {
        var key = GenerateCacheKey(Args(windowPeriods, offsetPeriods, pointType));
        return CachedFunc(key, () => prices.GetRollingPivots(windowPeriods, offsetPeriods, pointType));
    }

    public IEnumerable<RsiResult> GetRsi(int lookbackPeriods = 14)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetRsi(lookbackPeriods));
    }

    public IEnumerable<SlopeResult> GetSlope(int lookbackPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetSlope(lookbackPeriods));
    }

    public IEnumerable<SmiResult> GetSmi(int lookbackPeriods = 13,
        int firstSmoothPeriods = 25,
        int secondSmoothPeriods = 2,
        int signalPeriods = 3)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods));
        return CachedFunc(key,
            () => prices.GetSmi(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods));
    }

    public IEnumerable<SmmaResult> GetSmma(int lookbackPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetSmma(lookbackPeriods));
    }

    public IEnumerable<StarcBandsResult> GetStarcBands(int smaPeriods,
        double multiplier = 2.0,
        int atrPeriods = 10)
    {
        var key = GenerateCacheKey(Args(smaPeriods, multiplier, atrPeriods));
        return CachedFunc(key, () => prices.GetStarcBands(smaPeriods, multiplier, atrPeriods));
    }

    public IEnumerable<StcResult> GetStc(int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
    {
        var key = GenerateCacheKey(Args(cyclePeriods, fastPeriods, slowPeriods));
        return CachedFunc(key, () => prices.GetStc(cyclePeriods, fastPeriods, slowPeriods));
    }

    public IEnumerable<StdDevResult> GetStdDev(int lookbackPeriods,
        int? smaPeriods = null)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, smaPeriods));
        return CachedFunc(key, () => prices.GetStdDev(lookbackPeriods, smaPeriods));
    }

    public IEnumerable<StdDevChannelsResult> GetStdDevChannels(int? lookbackPeriods = 20,
        double stdDeviations = 2.0)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, stdDeviations));
        return CachedFunc(key, () => prices.GetStdDevChannels(lookbackPeriods, stdDeviations));
    }

    public IEnumerable<StochResult> GetStoch(int lookbackPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 3)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, signalPeriods, smoothPeriods));
        return CachedFunc(key, () => prices.GetStoch(lookbackPeriods, signalPeriods, smoothPeriods));
    }

    public IEnumerable<StochRsiResult> GetStochRsi(int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods = 1)
    {
        var key = GenerateCacheKey(Args(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods));
        return CachedFunc(key, () => prices.GetStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods));
    }

    public IEnumerable<SuperTrendResult> GetSuperTrend(int lookbackPeriods = 10,
        double multiplier = 3.0)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, multiplier));
        return CachedFunc(key, () => prices.GetSuperTrend(lookbackPeriods, multiplier));
    }

    public IEnumerable<T3Result> GetT3(int lookbackPeriods = 5,
        double volumeFactor = 0.7)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, volumeFactor));
        return CachedFunc(key, () => prices.GetT3(lookbackPeriods, volumeFactor));
    }

    public IEnumerable<TemaResult> GetTema(int lookbackPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetTema(lookbackPeriods));
    }

    public IEnumerable<TrResult> GetTr()
    {
        var key = GenerateCacheKey(Args());
        return CachedFunc(key, () => prices.GetTr());
    }

    public IEnumerable<TrixResult> GetTrix(int lookbackPeriods,
        int? signalPeriods = null)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, signalPeriods));
        return CachedFunc(key, () => prices.GetTrix(lookbackPeriods, signalPeriods));
    }

    public IEnumerable<TsiResult> GetTsi(int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, smoothPeriods, signalPeriods));
        return CachedFunc(key, () => prices.GetTsi(lookbackPeriods, smoothPeriods, signalPeriods));
    }

    public IEnumerable<UlcerIndexResult> GetUlcerIndex(int lookbackPeriods = 14)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetUlcerIndex(lookbackPeriods));
    }

    public IEnumerable<UltimateResult> GetUltimate(int shortPeriods = 7,
        int middlePeriods = 14,
        int longPeriods = 28)
    {
        var key = GenerateCacheKey(Args(shortPeriods, middlePeriods, longPeriods));
        return CachedFunc(key, () => prices.GetUltimate(shortPeriods, middlePeriods, longPeriods));
    }

    public IEnumerable<VolatilityStopResult> GetVolatilityStop(int lookbackPeriods = 7,
        double multiplier = 3.0)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods, multiplier));
        return CachedFunc(key, () => prices.GetVolatilityStop(lookbackPeriods, multiplier));
    }

    public IEnumerable<VortexResult> GetVortex(int lookbackPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetVortex(lookbackPeriods));
    }

    public IEnumerable<VwapResult> GetVwap(DateTime? startDate = null)
    {
        var key = GenerateCacheKey(Args(startDate));
        return CachedFunc(key, () => prices.GetVwap(startDate));
    }

    public IEnumerable<VwmaResult> GetVwma(int lookbackPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetVwma(lookbackPeriods));
    }

    public IEnumerable<WilliamsResult> GetWilliamsR(int lookbackPeriods = 14)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetWilliamsR(lookbackPeriods));
    }

    public IEnumerable<WmaResult> GetWma(int lookbackPeriods)
    {
        var key = GenerateCacheKey(Args(lookbackPeriods));
        return CachedFunc(key, () => prices.GetWma(lookbackPeriods));
    }

    public IEnumerable<ZigZagResult> GetZigZag(EndType endType = EndType.Close,
        Decimal percentChange = 5M)
    {
        var key = GenerateCacheKey(Args(endType, percentChange));
        return CachedFunc(key, () => prices.GetZigZag(endType, percentChange));
    }
}