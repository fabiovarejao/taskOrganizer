using TaskOrganizer.Domain.Entities;

namespace TaskOrganizer.Domain.Interfaces.Repositories;

public interface ITaskUserRepository
{
    Task<TaskUser?> GetByIdAsync(Guid taskId, Guid userId);
    Task<IEnumerable<TaskUser>> GetByTaskIdAsync(Guid taskId);
    Task<IEnumerable<TaskUser>> GetByUserIdAsync(Guid userId);
    Task AddAsync(TaskUser taskUser);
    Task DeleteAsync(TaskUser taskUser);
    Task<bool> ExistsAsync(Guid taskId, Guid userId);
}
