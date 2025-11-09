using AutoMapper;
using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Application.Interfaces;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Enums;
using TaskOrganizer.Domain.Interfaces.Repositories;
using TaskStatus = TaskOrganizer.Domain.Enums.TaskStatus;

namespace TaskOrganizer.Application.Services;

public class TaskService : Interfaces.ITaskService
{
    private readonly IProjectRepository _projects;
    private readonly ITaskRepository _tasks;
    private readonly IMapper _mapper;

    public TaskService(IProjectRepository projects, ITaskRepository tasks, IMapper mapper)
    {
        _projects = projects;
        _tasks = tasks;
        _mapper = mapper;
    }

    public async Task<TaskDto?> CreateTaskAsync(Guid projectId, CreateTaskDto dto)
    {
        var project = await _projects.GetByIdAsync(projectId);
        if (project is null) return null;
        // Enforce task limit (already enforced inside Project.AddTask but we double check for clarity)
        if (project.Tasks.Count >= project.TaskLimit)
            throw new Domain.Exceptions.DomainException($"O projeto atingiu o número máximo de tarefas ({project.TaskLimit}).");

        var task = project.AddTask(dto.Title, dto.Priority, dto.ResponsibleUserId, dto.Description, dto.DueDate);
        await _tasks.AddAsync(task);
        await _tasks.SaveChangesAsync();
        return _mapper.Map<TaskDto>(task);
    }

    public async Task<IEnumerable<TaskDto>> GetTasksForProjectAsync(Guid projectId)
    {
        var q = _tasks.Query().Where(t => t.ProjectId == projectId);
        var list = q.ToList();
        return _mapper.Map<IEnumerable<TaskDto>>(list);
    }

    public async Task<TaskDto?> UpdateStatusAsync(Guid taskId, TaskStatus newStatus, Guid userId)
    {
        var task = await _tasks.GetByIdAsync(taskId);
        if (task is null) return null;
        
        task.UpdateStatus(newStatus, userId);
        
        // No explicit Update needed - EF tracks changes to tracked entities and new child entities
        await _tasks.SaveChangesAsync();
        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto?> UpdateTaskAsync(Guid taskId, UpdateTaskDto dto, Guid userId)
    {
        var task = await _tasks.GetByIdAsync(taskId);
        if (task is null) return null;

        // Business Rule #1: Priority cannot be changed after creation
        if (dto.Priority.HasValue && dto.Priority.Value != task.Priority)
        {
            throw new Domain.Exceptions.DomainException(
                "A prioridade da tarefa não pode ser alterada após a criação.");
        }

        // Update fields that have changed and record history
        if (task.Title != dto.Title)
            task.UpdateTitle(dto.Title, userId);

        if (task.Description != dto.Description)
            task.UpdateDescription(dto.Description, userId);

        if (task.DueDate != dto.DueDate)
            task.UpdateDueDate(dto.DueDate, userId);

        if (dto.Status.HasValue && task.Status != dto.Status.Value)
            task.UpdateStatus(dto.Status.Value, userId);
        
        // No explicit Update needed - EF tracks property changes and new child entities
        await _tasks.SaveChangesAsync();
        return _mapper.Map<TaskDto>(task);
    }

    public async Task AddCommentAsync(Guid taskId, string message, Guid userId)
    {
        var task = await _tasks.GetByIdAsync(taskId);
        if (task is null) throw new KeyNotFoundException();
        
        task.AddComment(message, userId);
        
        // No explicit Update needed - EF tracks new child entities automatically
        await _tasks.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(Guid taskId)
    {
        var task = await _tasks.GetByIdAsync(taskId);
        if (task is null) throw new KeyNotFoundException($"Task with ID {taskId} not found");
        _tasks.Remove(task);
        await _tasks.SaveChangesAsync();
    }
}
