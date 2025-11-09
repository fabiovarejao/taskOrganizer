namespace TaskOrganizer.Application.Dtos;

public record CompletedPerUserResult(Guid UserId, string? UserName, int CompletedCount, double AveragePerDay);
