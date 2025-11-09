using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Domain.Enums;
using TaskStatus = TaskOrganizer.Domain.Enums.TaskStatus;

namespace TaskOrganizer.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetTasksForProjectAsync(Guid projectId);
    Task<TaskDto?> CreateTaskAsync(Guid projectId, CreateTaskDto dto);
    Task<TaskDto?> UpdateTaskAsync(Guid taskId, UpdateTaskDto dto, Guid userId);
    Task<TaskDto?> UpdateStatusAsync(Guid taskId, TaskStatus newStatus, Guid userId);
    Task AddCommentAsync(Guid taskId, string message, Guid userId);
    Task DeleteTaskAsync(Guid taskId);
}
