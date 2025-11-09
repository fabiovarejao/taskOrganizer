namespace TaskOrganizer.Application.Dtos;

/// <summary>
/// Data transfer object for TaskComment entity.
/// </summary>
public record TaskCommentDto(
    Guid Id,
    Guid TaskId,
    string Message,
    Guid UserId,
    DateTime CreatedAt);
