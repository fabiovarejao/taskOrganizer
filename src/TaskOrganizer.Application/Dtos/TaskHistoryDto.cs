namespace TaskOrganizer.Application.Dtos;

/// <summary>
/// Data transfer object for TaskHistory entity.
/// </summary>
public record TaskHistoryDto(
    Guid Id,
    Guid TaskId,
    string FieldChanged,
    string? OldValue,
    string? NewValue,
    DateTime ChangedAt,
    Guid ChangedByUserId);
