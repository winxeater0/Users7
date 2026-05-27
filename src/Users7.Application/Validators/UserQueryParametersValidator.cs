using FluentValidation;
using Users7.Application.DTOs;

namespace Users7.Application.Validators;

public sealed class UserQueryParametersValidator : AbstractValidator<UserQueryParameters>
{
    private static readonly string[] AllowedSortFields = ["id", "name", "email", "birthDate", "createdAt", "updatedAt"];
    private static readonly string[] AllowedSortDirections = ["asc", "desc"];

    public UserQueryParametersValidator()
    {
        RuleFor(parameters => parameters.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Número da página deve ser maior ou igual a 1.");

        RuleFor(parameters => parameters.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Tamanho da página deve estar entre 1 e 100.");

        RuleFor(parameters => parameters.SortBy)
            .Must(sortBy => AllowedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Campo de ordenação inválido.");

        RuleFor(parameters => parameters.SortDirection)
            .Must(direction => AllowedSortDirections.Contains(direction, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Direção de ordenação inválida.");
    }
}
