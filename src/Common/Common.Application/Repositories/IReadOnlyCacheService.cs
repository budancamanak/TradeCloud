namespace Common.Application.Repositories;

public interface IReadOnlyCacheService
{ 
    Task<T?> GetAsync<T>(string key);
}