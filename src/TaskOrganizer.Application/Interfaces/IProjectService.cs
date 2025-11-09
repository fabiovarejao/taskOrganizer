using TaskOrganizer.Application.Dtos;

namespace TaskOrganizer.Application.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetByUserAsync(Guid userId);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto);
    Task DeleteAsync(Guid projectId);
    Task<ProjectDto?> GetByIdAsync(Guid projectId);
    Task<ProjectDto?> UpdateAsync(Guid projectId, UpdateProjectDto dto);
}
