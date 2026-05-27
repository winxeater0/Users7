using FluentValidation;
using Users7.Application.DTOs;
using Users7.Application.Interfaces;

namespace Users7.Application.Validators;

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator(IDateProvider dateProvider)
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(50).WithMessage("Nome deve ter no máximo 50 caracteres.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail deve ser válido.")
            .MaximumLength(254).WithMessage("E-mail deve ter no máximo 254 caracteres.");

        RuleFor(user => user.BirthDate)
            .Must(birthDate => birthDate <= dateProvider.Today)
            .WithMessage("Data de nascimento não pode ser futura.");
    }
}
