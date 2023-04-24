using Cassino.Domain.Entities;
using FluentValidation;

namespace Cassino.Domain.Validation;

public class UsuarioValidator: AbstractValidator<Usuario>
{
    public UsuarioValidator()
    {
        RuleFor(u => u.Nome)
            .NotEmpty();

        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(u => u.Cpf)
            .NotEmpty();
        
        RuleFor(u => u.Senha)
            .NotEmpty();

        RuleFor(u => u.Desativado)
            .NotNull();

        RuleFor(u => u.ChavePix)
            .NotEmpty();
    }
}