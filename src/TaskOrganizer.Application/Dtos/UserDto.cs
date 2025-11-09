using TaskOrganizer.Domain.Enums;

namespace TaskOrganizer.Application.Dtos;

/// <summary>
/// Data transfer object for User entity.
/// </summary>
public record UserDto(
    Guid Id,
    string UserName,
    Position Position);
