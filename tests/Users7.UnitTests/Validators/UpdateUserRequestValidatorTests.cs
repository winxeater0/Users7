using FluentAssertions;
using Users7.Application.DTOs;
using Users7.Application.Validators;
using Users7.UnitTests.Fakes;

namespace Users7.UnitTests.Validators;

public sealed class UpdateUserRequestValidatorTests
{
    private readonly UpdateUserRequestValidator validator = new(new FakeDateProvider(new DateOnly(2026, 5, 26)));

    [Fact]
    public void Validate_ShouldFail_WhenNameExceedsLimit()
    {
        var request = new UpdateUserRequest(new string('A', 51), "maria@example.com", new DateOnly(1990, 5, 20));

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "Nome deve ter no máximo 50 caracteres.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailExceedsLimit()
    {
        var email = $"{new string('a', 245)}@example.com";
        var request = new UpdateUserRequest("Maria Silva", email, new DateOnly(1990, 5, 20));

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "E-mail deve ter no máximo 254 caracteres.");
    }

    [Fact]
    public void Validate_ShouldPass_WhenRequestIsValid()
    {
        var request = new UpdateUserRequest("Maria Silva", "maria@example.com", new DateOnly(1990, 5, 20));

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }
}
