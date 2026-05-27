namespace Users7.Application.DTOs;

public sealed record UpdateUserRequest(string Name, string Email, DateOnly BirthDate);
