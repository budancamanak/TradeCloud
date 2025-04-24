using Common.Application.Repositories;
using Common.Core.Models;
using Security.Domain.Entities;

namespace Security.Application.Abstraction.Repositories;

public interface IUserRepository : IAsyncRepository<User>
{

    Task<MethodResponse> CheckUsernameAvailability(string username);
    Task<MethodResponse> UpdateUserStatus(string username);
}