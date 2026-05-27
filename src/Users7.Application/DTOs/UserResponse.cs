namespace Users7.Application.DTOs;

public sealed record UserResponse(
    int Id,
    string Name,
    string Email,
    DateOnly BirthDate,
    DateOnly CreatedAt,
    DateOnly UpdatedAt);
