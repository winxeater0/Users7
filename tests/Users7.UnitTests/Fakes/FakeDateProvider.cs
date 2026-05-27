using Users7.Application.Interfaces;

namespace Users7.UnitTests.Fakes;

public sealed class FakeDateProvider : IDateProvider
{
    public FakeDateProvider(DateOnly today)
    {
        Today = today;
    }

    public DateOnly Today { get; }
}
