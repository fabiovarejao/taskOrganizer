using AutoMapper;
using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Application.Interfaces;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Interfaces.Repositories;

namespace TaskOrganizer.Application.Services;

public class ProjectService : Interfaces.IProjectService
{
    private readonly IProjectRepository _projects;
    private readonly IMapper _mapper;

    public ProjectService(IProjectRepository projects, IMapper mapper)
    {
        _projects = projects;
        _mapper = mapper;
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
    {
        var project = new Project()
        {
            Name = dto.Name,
            Description = dto.Description,
            UserId = dto.UserId
        };
        await _projects.AddAsync(project);
        await _projects.SaveChangesAsync();
        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<IEnumerable<ProjectDto>> GetByUserAsync(Guid userId)
    {
        var q = _projects.Query().Where(p => p.UserId == userId).ToList();
        return _mapper.Map<IEnumerable<ProjectDto>>(q);
    }

    public async Task DeleteAsync(Guid projectId)
    {
        var project = await _projects.GetByIdAsync(projectId);
        if (project is null) return;

        // Business rule: Cannot delete project with pending tasks
        if (project.HasPendingTasks())
        {
            throw new Domain.Exceptions.DomainException("O Projeto tem tarefas pendentes e não pode ser removido. Remova ou conclua todas as tarefas antes de excluir o projeto.");
        }

        _projects.Remove(project);
        await _projects.SaveChangesAsync();
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid projectId)
    {
        var project = await _projects.GetByIdAsync(projectId);
        return project is null ? null : _mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto?> UpdateAsync(Guid projectId, UpdateProjectDto dto)
    {
        var project = await _projects.GetByIdAsync(projectId);
        if (project is null) return null;

        // Simple updates - preserve business rules
        var nameProp = project.GetType().GetProperty("Name");
        var descProp = project.GetType().GetProperty("Description");
        nameProp!.SetValue(project, dto.Name);
        descProp!.SetValue(project, dto.Description);

        await _projects.SaveChangesAsync();
        return _mapper.Map<ProjectDto>(project);
    }
}
