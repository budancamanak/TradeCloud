namespace Common.Core.Extensions;

public static class ConversionExtensions
{
    public static int ToInt(this object? obj, int defaultValue = 0)
    {
        if (obj == null) return defaultValue;
        if (obj is int i) return i;
        if (int.TryParse(obj.ToString(), out int result))
        {
            return result;
        }

        return 0;
    }

    public static double ToDouble(this object? obj, double defaultValue = 0)
    {
        if (obj == null) return defaultValue;
        if (obj is double i) return i;
        if (double.TryParse(obj.ToString(), out double result))
        {
            return result;
        }

        return 0;
    }
}