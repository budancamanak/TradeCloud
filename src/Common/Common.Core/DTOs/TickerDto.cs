namespace Common.Core.DTOs;

public record TickerDto(int Id, string Name, string Symbol, string ExchangeName, int DecimalPoint)
{
}