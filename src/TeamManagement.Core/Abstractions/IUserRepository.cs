using TeamManagement.Core.Entities;

namespace TeamManagement.Core.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByUserNameAsync(string userName);
    Task<int> AddAsync(User user);
    Task<bool> UpdateAsync(User user);
}