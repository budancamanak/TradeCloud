using Skender.Stock.Indicators;

namespace Common.Plugin.Math;

public record ReusableResult(double? Value, DateTime Date) : IReusableResult;

public static class TradeMathEx
{
    public static CandlePart ToCandlePart(this string value)
    {
        return Enum.TryParse(value, true, out CandlePart candle) ? candle : CandlePart.Close;
    }

    public static MaType ToMaType(this string value)
    {
        return Enum.TryParse(value, true, out MaType ma) ? ma : MaType.SMA;
    }

    internal static IReusableResult Extract(this IQuote quote, CandlePart candle)
    {
        return candle switch
        {
            CandlePart.Open => new ReusableResult(quote.Open.ToDouble(), quote.Date),
            CandlePart.High => new ReusableResult(quote.High.ToDouble(), quote.Date),
            CandlePart.Low => new ReusableResult(quote.Low.ToDouble(), quote.Date),
            CandlePart.Close => new ReusableResult(quote.Close.ToDouble(), quote.Date),
            CandlePart.Volume => new ReusableResult(quote.Volume.ToDouble(), quote.Date),
            CandlePart.HL2 => new ReusableResult(((quote.High + quote.Low) / 2).ToDouble(), quote.Date),
            CandlePart.HLC3 => new ReusableResult(((quote.High + quote.Low + quote.Close) / 3).ToDouble(), quote.Date),
            CandlePart.OC2 => new ReusableResult(((quote.Open + quote.Close) / 2).ToDouble(), quote.Date),
            CandlePart.OHL3 => new ReusableResult(((quote.Open + quote.High + quote.Low) / 3).ToDouble(), quote.Date),
            CandlePart.OHLC4 => new ReusableResult(((quote.Open + quote.High + quote.Low + quote.Close) / 4).ToDouble(),
                quote.Date),
            _ => new ReusableResult(quote.Close.ToDouble(), quote.Date)
        };
    }

    public static List<IReusableResult> Extract(this IEnumerable<IQuote> quotes, string src)
    {
        var candle = src.ToCandlePart();
        return quotes.Select(s => s.Extract(candle)).ToList();
    }

    public static IEnumerable<ReusableResult> Diff(IEnumerable<IReusableResult> s1, IEnumerable<IReusableResult> s2,
        bool abs)
    {
        List<IReusableResult> first = s1.ToList(), second = s2.ToList();
        int bound = System.Math.Min(first.Count, second.Count);
        for (int i = 0; i < bound; i++)
        {
            var val = first[i].Value - second[i].Value;
            if (!val.HasValue) continue;
            if (abs) val = System.Math.Abs(val.Value);
            yield return new ReusableResult(val.Value, first[i].Date);
        }
    }

    public static IEnumerable<ReusableResult> Divide(IEnumerable<IReusableResult> s1, IEnumerable<IReusableResult> s2)
    {
        List<IReusableResult> first = s1.ToList(), second = s2.ToList();
        int bound = System.Math.Min(first.Count, second.Count);
        for (int i = 0; i < bound; i++)
        {
            var secondVal = second[i].Value;
            if (secondVal == 0) secondVal = 1;
            var val = first[i].Value / secondVal;
            if (!val.HasValue) continue;
            yield return new ReusableResult(val.Value, first[i].Date);
        }
    }

    public static IEnumerable<ReusableResult> Multiply(IEnumerable<IReusableResult> s1, IEnumerable<IReusableResult> s2)
    {
        List<IReusableResult> first = s1.ToList(), second = s2.ToList();
        int bound = System.Math.Min(first.Count, second.Count);
        for (int i = 0; i < bound; i++)
        {
            var val = first[i].Value * second[i].Value;
            if (!val.HasValue) continue;
            yield return new ReusableResult(val.Value, first[i].Date);
        }
    }

    public static IEnumerable<ReusableResult> Multiply(IEnumerable<IReusableResult> s1, double s2)
    {
        List<IReusableResult> first = s1.ToList();
        for (int i = 0; i < first.Count; i++)
        {
            var val = first[i].Value * s2;
            if (!val.HasValue) continue;
            yield return new ReusableResult(val.Value, first[i].Date);
        }
    }

    public static IEnumerable<double> Sum(IEnumerable<IReusableResult> s1, IEnumerable<IReusableResult> s2)
    {
        List<IReusableResult> first = s1.ToList(), second = s2.ToList();
        int bound = System.Math.Min(first.Count, second.Count);
        for (int i = 0; i < bound; i++)
        {
            var val = first[i].Value + second[i].Value;
            if (!val.HasValue) continue;
            yield return val.Value;
        }
    }

    public static IEnumerable<IReusableResult> CalculateMa(this List<Quote> quotes, string maType, string src,
        int lookbackPeriods)
    {
        var ma = maType.ToMaType();
        var candle = src.ToCandlePart();
        var tuples = quotes.ToTuple<IQuote>(candle);
        switch (ma)
        {
            case MaType.ALMA:
                break;
            case MaType.DEMA:
                break;
            case MaType.EPMA:
                break;
            case MaType.EMA:
                return tuples.CalculateMa(lookbackPeriods);
            case MaType.HMA:
                break;
            case MaType.KAMA:
                break;
            case MaType.MAMA:
                break;
            case MaType.SMA:
                break;
            case MaType.SMMA:
                break;
            case MaType.TEMA:
                break;
            case MaType.WMA:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // return .CalcEma(lookbackPeriods);
        return null;
    }

    public static List<(DateTime, double)> ToTuple<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote => quotes
        .OrderBy(x => x.Date)
        .Select(x => x.ToTuple(candlePart))
        .ToList();

    public static (DateTime date, double value) ToTuple<TQuote>(
        this TQuote q,
        CandlePart candlePart)
        where TQuote : IQuote => candlePart switch
    {
        CandlePart.Open => (q.Date, (double)q.Open),
        CandlePart.High => (q.Date, (double)q.High),
        CandlePart.Low => (q.Date, (double)q.Low),
        CandlePart.Close => (q.Date, (double)q.Close),
        CandlePart.Volume => (q.Date, (double)q.Volume),
        CandlePart.HL2 => (q.Date, (double)(q.High + q.Low) / 2),
        CandlePart.HLC3 => (q.Date, (double)(q.High + q.Low + q.Close) / 3),
        CandlePart.OC2 => (q.Date, (double)(q.Open + q.Close) / 2),
        CandlePart.OHL3 => (q.Date, (double)(q.Open + q.High + q.Low) / 3),
        CandlePart.OHLC4 => (q.Date, (double)(q.Open + q.High + q.Low + q.Close) / 4),
        _ => throw new ArgumentOutOfRangeException(nameof(candlePart), candlePart, "Invalid candlePart provided."),
    };

    private static (DateTime date, double value) ToTuple(this IReusableResult item) =>
        (item.Date, item.Value.GetValueOrDefault());


    public static List<(DateTime, double )> ToTuple(this IEnumerable<IReusableResult> list)
    {
        return list.OrderBy(f => f.Date).Select(s => s.ToTuple()).ToList();
    }

    public static double EmaIncrement(double newValue, double lastEma, double k)
        => lastEma + (k * (newValue - lastEma));

    public static List<EmaResult> CalculateMa(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        int count = tpList.Count;
        List<EmaResult> emaResultList = new List<EmaResult>(count);
        double num1 = 0.0;
        double k = 2.0 / (double)checked(lookbackPeriods + 1);
        int num2 = System.Math.Min(lookbackPeriods, count);
        int index1 = 0;
        while (index1 < num2)
        {
            double num3 = tpList[index1].Item2;
            num1 += num3;
            checked
            {
                ++index1;
            }
        }

        double lastEma = num1 / (double)lookbackPeriods;
        int index2 = 0;
        while (index2 < count)
        {
            (DateTime date, double newValue) = tpList[index2];
            EmaResult emaResult = new EmaResult(date);
            emaResultList.Add(emaResult);
            if (checked(index2 + 1) > lookbackPeriods)
            {
                double num4 = EmaIncrement(newValue, lastEma, k);
                emaResult.Ema = num4.NaN2Null();
                lastEma = num4;
            }
            else if (index2 == checked(lookbackPeriods - 1))
                emaResult.Ema = lastEma.NaN2Null();

            checked
            {
                ++index2;
            }
        }

        return emaResultList;
    }
}