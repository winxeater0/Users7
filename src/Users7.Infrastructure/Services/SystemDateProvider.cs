using Users7.Application.Interfaces;

namespace Users7.Infrastructure.Services;

public sealed class SystemDateProvider : IDateProvider
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
