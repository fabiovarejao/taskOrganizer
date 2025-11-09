using TaskOrganizer.Domain.Enums;
using TaskStatus = TaskOrganizer.Domain.Enums.TaskStatus;

namespace TaskOrganizer.Application.Dtos;

/// <summary>
/// DTO for updating task details. Priority is optional but if provided must match original (immutable rule).
/// </summary>
public record UpdateTaskDto(
    string Title, 
    string? Description, 
    DateTime? DueDate, 
    TaskPriority? Priority, 
    TaskStatus? Status);
