using TaskOrganizer.Domain.Entities;

namespace TaskOrganizer.Domain.Interfaces.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task AddAsync(TaskItem task);
    void Update(TaskItem task);
    void Remove(TaskItem task);
    IQueryable<TaskItem> Query();
    Task SaveChangesAsync();
}
