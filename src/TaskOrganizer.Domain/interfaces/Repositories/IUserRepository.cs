using TaskOrganizer.Domain.Entities;

namespace TaskOrganizer.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByUserNameAsync(string userName);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<bool> ExistsAsync(Guid id);
}
