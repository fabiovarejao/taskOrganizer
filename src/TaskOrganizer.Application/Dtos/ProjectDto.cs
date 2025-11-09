namespace TaskOrganizer.Application.Dtos;

/// <summary>
/// Data transfer object for Project entity.
/// </summary>
public record ProjectDto(
    Guid Id, 
    string Name, 
    string? Description, 
    Guid UserId,
    int TaskCount);
