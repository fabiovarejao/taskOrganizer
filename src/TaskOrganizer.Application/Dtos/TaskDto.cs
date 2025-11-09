using TaskOrganizer.Domain.Enums;
using TaskStatus = TaskOrganizer.Domain.Enums.TaskStatus;

namespace TaskOrganizer.Application.Dtos;

/// <summary>
/// Data transfer object for Task entity.
/// </summary>
public record TaskDto(
    Guid Id, 
    string Title, 
    string? Description, 
    DateTime? DueDate, 
    TaskStatus Status, 
    TaskPriority Priority,
    Guid ProjectId,
    Guid? ResponsibleUserId,
    List<TaskHistoryDto>? History,
    List<TaskCommentDto>? Comments);
