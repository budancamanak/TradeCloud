using Common.Core.Models;

namespace Common.Application.Repositories
{
    public interface IAsyncRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<MethodResponse> AddAsync(T item);
        Task<MethodResponse> UpdateAsync(T item);
        Task<MethodResponse> DeleteAsync(T item);
        Task<MethodResponse> DeleteAsync(int id);
    }
}