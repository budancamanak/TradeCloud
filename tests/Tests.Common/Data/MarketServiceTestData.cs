using Common.Core.Enums;
using Market.Domain.Entities;

namespace Tests.Common.Data;

public class MarketServiceTestData
{
    public static DateTime Now = DateTime.UtcNow;

    public List<Exchange> Exchanges { get; } = new List<Exchange>
    {
        new Exchange { ConnectionUrl = "", Id = 1, Name = "Binance", Tickers = [] },
        new Exchange { ConnectionUrl = "", Id = 2, Name = "OKX", Tickers = [] }
    };

    public List<Ticker> Tickers { get; } = new List<Ticker>()
    {
        new Ticker { DecimalPoint = 1, ExchangeId = 1, Id = 1, Name = "Bitcoin", Symbol = "BTC/USDT", Prices = [] },
        new Ticker { DecimalPoint = 2, ExchangeId = 1, Id = 2, Name = "Ethereum", Symbol = "ETH/USDT", Prices = [] },
        new Ticker { DecimalPoint = 1, ExchangeId = 1, Id = 3, Name = "Ripple", Symbol = "XRP/USDT", Prices = [] },
        new Ticker { DecimalPoint = 3, ExchangeId = 2, Id = 4, Name = "ChainLink", Symbol = "LINK/USDT", Prices = [] },
        new Ticker { DecimalPoint = 4, ExchangeId = 2, Id = 5, Name = "Polkadot", Symbol = "DOT/USDT", Prices = [] },
    };

    public List<Price> Prices { get; } = new List<Price>()
    {
        new Price
        {
            TickerId = 1, Close = 101000, High = 1001101, Low = 99989, Open = 99979, Timeframe = Timeframe.Hour1,
            Timestamp = Now.AddHours(-1)
        },
        new Price
        {
            TickerId = 1, Close = 100000, High = 1000001, Low = 99999, Open = 99999, Timeframe = Timeframe.Hour1,
            Timestamp = Now.AddHours(-2)
        },
        new Price
        {
            TickerId = 1, Close = 100020, High = 1000201, Low = 99979, Open = 99969, Timeframe = Timeframe.Hour1,
            Timestamp = Now.AddHours(-3)
        },
        new Price
        {
            TickerId = 1, Close = 100070, High = 1000111, Low = 99989, Open = 99999, Timeframe = Timeframe.Hour1,
            Timestamp = Now.AddHours(-4)
        },
        new Price
        {
            TickerId = 1, Close = 100000, High = 1000001, Low = 99969, Open = 99999, Timeframe = Timeframe.Hour1,
            Timestamp = Now.AddHours(-5)
        },
        new Price
        {
            TickerId = 2, Close = 3000, High = 3020, Low = 3000, Open = 3005, Timeframe = Timeframe.Hour1,
            Timestamp = Now.AddHours(-1)
        },
        new Price
        {
            TickerId = 2, Close = 3000, High = 3020, Low = 3000, Open = 3005, Timeframe = Timeframe.Hour1,
            Timestamp = Now.AddHours(-2)
        },
        new Price
        {
            TickerId = 2, Close = 3000, High = 3020, Low = 3000, Open = 3005, Timeframe = Timeframe.Hour1,
            Timestamp = Now.AddHours(-3)
        },
        new Price
        {
            TickerId = 2, Close = 3000, High = 3020, Low = 3000, Open = 3005, Timeframe = Timeframe.Hour1,
            Timestamp = Now.AddHours(-4)
        },
        new Price
        {
            TickerId = 2, Close = 3000, High = 3020, Low = 3000, Open = 3005, Timeframe = Timeframe.Hour1,
            Timestamp = Now.AddHours(-5)
        },
    }.OrderBy(f => f.Timestamp).ToList();

    public static MarketServiceTestData Instance { get; } = new();

    private MarketServiceTestData()
    {
        foreach (var ticker in Tickers)
        {
            ticker.Prices = Prices.Where(f => f.TickerId == ticker.Id).ToList();
            ticker.Prices.ForEach(f => f.Ticker = ticker);
        }

        foreach (var exchange in Exchanges)
        {
            exchange.Tickers = Tickers.Where(f => f.ExchangeId == exchange.Id).ToList();
            exchange.Tickers.ForEach(f => f.Exchange = exchange);
        }
    }
}