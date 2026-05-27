namespace Users7.Api.Responses;

public sealed record ApiResponse<T>(
    string Code,
    string Message,
    T? Data = default,
    string[]? Errors = null,
    object? Meta = null,
    string? TraceId = null);
