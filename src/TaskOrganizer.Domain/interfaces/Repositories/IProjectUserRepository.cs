using TaskOrganizer.Domain.Entities;

namespace TaskOrganizer.Domain.Interfaces.Repositories;

public interface IProjectUserRepository
{
    Task<ProjectUser?> GetByIdAsync(Guid projectId, Guid userId);
    Task<IEnumerable<ProjectUser>> GetByProjectIdAsync(Guid projectId);
    Task<IEnumerable<ProjectUser>> GetByUserIdAsync(Guid userId);
    Task AddAsync(ProjectUser projectUser);
    Task DeleteAsync(ProjectUser projectUser);
    Task<bool> ExistsAsync(Guid projectId, Guid userId);
}
