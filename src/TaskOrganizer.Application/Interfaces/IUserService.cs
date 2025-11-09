using TaskOrganizer.Application.Dtos;

namespace TaskOrganizer.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
}
