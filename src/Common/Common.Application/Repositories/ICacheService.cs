namespace Common.Application.Repositories;

public interface ICacheService : IReadOnlyCacheService
{
    Task SetAsync<T>(string key, T value, TimeSpan expiration);
    Task RemoveAsync(string key);
    Task<double> Increment(string key,double value);
    Task<double> Decrement(string key,double value);
}