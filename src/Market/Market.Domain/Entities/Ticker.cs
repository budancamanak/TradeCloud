namespace Market.Domain.Entities;

public class Ticker
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public int DecimalPoint { get; set; }
    public int ExchangeId { get; set; }
    public Exchange Exchange { get; set; }
    public List<Price> Prices { get; set; }

    public override string ToString()
    {
        return $"[{Id}]: {Name}({Symbol}) - Exchange({Exchange.Name})";
    }
}