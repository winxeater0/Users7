namespace Users7.Application.DTOs;

public sealed class UserQueryParameters
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string SortBy { get; init; } = "id";
    public string SortDirection { get; init; } = "asc";
}
