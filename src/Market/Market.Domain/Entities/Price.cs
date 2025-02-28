using Common.Core.Attributes;
using Common.Core.Enums;
using Common.Core.Extensions;

namespace Market.Domain.Entities;

public class Price
{
    [HypertableColumn] public DateTime Timestamp { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Open { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
    public int TickerId { get; set; }
    public Timeframe Timeframe { get; set; }
    public Ticker Ticker { get; set; }

    public override string ToString()
    {
        return $"[{Timestamp.ToReadableString()}- Open:{Open} High:{High} Close:{Close} Low:{Low}]";
    }
}