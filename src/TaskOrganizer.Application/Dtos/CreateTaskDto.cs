using TaskOrganizer.Domain.Enums;

namespace TaskOrganizer.Application.Dtos;

// ResponsibleUserId renamed from UserId to align with business terminology in README
public record CreateTaskDto(string Title, TaskPriority Priority, Guid ResponsibleUserId, string? Description, DateTime? DueDate);
