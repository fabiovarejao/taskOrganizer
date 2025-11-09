namespace TaskOrganizer.Application.Dtos;

public record CompletedPerUserDto(Guid UserId, string? UserName, int CompletedCount, double AveragePerDay);
