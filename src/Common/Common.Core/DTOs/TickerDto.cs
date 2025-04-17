namespace Common.Core.DTOs;

public record TickerDto(int Id, string Name, string Symbol, string ExchangeName, int DecimalPoint)
{
    public static TickerDto NULL_TICKER = new TickerDto(0, "NotFound", "NotFound", "", 0);
}