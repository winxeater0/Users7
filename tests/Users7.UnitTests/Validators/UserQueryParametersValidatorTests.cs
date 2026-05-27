using FluentAssertions;
using Users7.Application.DTOs;
using Users7.Application.Validators;

namespace Users7.UnitTests.Validators;

public sealed class UserQueryParametersValidatorTests
{
    private readonly UserQueryParametersValidator validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenPageNumberIsInvalid()
    {
        var parameters = new UserQueryParameters { PageNumber = 0 };

        var result = validator.Validate(parameters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "Número da página deve ser maior ou igual a 1.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPageSizeIsInvalid()
    {
        var parameters = new UserQueryParameters { PageSize = 101 };

        var result = validator.Validate(parameters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "Tamanho da página deve estar entre 1 e 100.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenSortFieldIsInvalid()
    {
        var parameters = new UserQueryParameters { SortBy = "password" };

        var result = validator.Validate(parameters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "Campo de ordenação inválido.");
    }

    [Fact]
    public void Validate_ShouldPass_WhenParametersAreValid()
    {
        var parameters = new UserQueryParameters { PageNumber = 1, PageSize = 10, SortBy = "createdAt", SortDirection = "desc" };

        var result = validator.Validate(parameters);

        result.IsValid.Should().BeTrue();
    }
}
