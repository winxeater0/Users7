namespace Users7.Application.DTOs;

public sealed record CreateUserRequest(string Name, string Email, DateOnly BirthDate);
