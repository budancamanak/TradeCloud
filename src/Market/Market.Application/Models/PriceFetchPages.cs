namespace Market.Application.Models;

public record PriceFetchPages(long Since, int Limit)
{
    public DateTime Date => DateTime.UnixEpoch.AddMilliseconds(Since);
}