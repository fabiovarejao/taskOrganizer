using TaskOrganizer.Domain.Entities;

namespace TaskOrganizer.Domain.Interfaces.Repositories;

public interface ITaskHistoryRepository
{
    Task<TaskHistory?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(Guid taskId);
    Task<IEnumerable<TaskHistory>> GetByUserIdAsync(Guid userId);
    Task AddAsync(TaskHistory taskHistory);
}
