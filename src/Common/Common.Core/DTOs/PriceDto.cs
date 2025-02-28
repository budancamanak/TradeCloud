using Common.Core.Extensions;

namespace Common.Core.DTOs;

public class PriceDto
{
    public DateTime Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Close { get; set; }
    public decimal Low { get; set; }
    public decimal Volume { get; set; }

    public PriceDto()
    {
    }

    public PriceDto(DateTime timestamp, decimal open, decimal high, decimal close, decimal low,decimal volume)
    {
        this.Timestamp = timestamp;
        this.Open = open;
        this.High = high;
        this.Low = low;
        this.Close = close;
        this.Volume = volume;
    }

    public override string ToString()
    {
        return $"[{Timestamp.ToReadableString()}- Open:{Open} High:{High} Close:{Close} Low:{Low}]";
    }
}