namespace Market.Domain.Entities;

public class Exchange
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? ConnectionUrl { get; set; }
    public List<Ticker> Tickers { get; set; }
    
    public override string ToString()
    {
        return $"[{Id}]: {Name} - {ConnectionUrl}";
    }
}