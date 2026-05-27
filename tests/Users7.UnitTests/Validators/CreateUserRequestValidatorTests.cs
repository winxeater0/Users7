using FluentAssertions;
using Users7.Application.DTOs;
using Users7.Application.Validators;
using Users7.UnitTests.Fakes;

namespace Users7.UnitTests.Validators;

public sealed class CreateUserRequestValidatorTests
{
    private readonly CreateUserRequestValidator validator = new(new FakeDateProvider(new DateOnly(2026, 5, 26)));

    [Fact]
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {
        var request = new CreateUserRequest(string.Empty, "maria@example.com", new DateOnly(1990, 5, 20));

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "Nome é obrigatório.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsInvalid()
    {
        var request = new CreateUserRequest("Maria Silva", "invalid-email", new DateOnly(1990, 5, 20));

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "E-mail deve ser válido.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenBirthDateIsFuture()
    {
        var request = new CreateUserRequest("Maria Silva", "maria@example.com", new DateOnly(2026, 5, 27));

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "Data de nascimento não pode ser futura.");
    }
}
