namespace Market.API.Models;

[Obsolete]
public class GetPricesForPluginQueryModel(
    int tickerId,
    string timeframe,
    DateTime startDate,
    DateTime endDate)
{
    public int TickerId { get; } = tickerId;
    public string Timeframe { get; } = timeframe;
    public DateTime StartDate { get; } = startDate;
    public DateTime EndDate { get; } = endDate;
}