using TaskOrganizer.Domain.Entities;

namespace TaskOrganizer.Domain.Interfaces.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    Task AddAsync(Project project);
    void Remove(Project project);
    IQueryable<Project> Query();
    Task SaveChangesAsync();
}
