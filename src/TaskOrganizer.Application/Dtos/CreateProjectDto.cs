namespace TaskOrganizer.Application.Dtos;

public record CreateProjectDto(string Name, Guid UserId, string? Description);
