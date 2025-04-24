using Common.Application.Repositories;
using Common.Core.Enums;
using Common.Core.Models;
using Security.Domain.Entities;

namespace Security.Application.Abstraction.Repositories;

public interface IUserRepository : IAsyncRepository<User>
{
    Task<List<User>> GetUsersWithStatus(Status status);
    Task<MethodResponse> CheckUsernameAvailability(string username);
    Task<MethodResponse> UpdateUserPassword(int id,string password);
    Task<MethodResponse> UpdateUserStatus(int id, Status status);
}