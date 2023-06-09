﻿using Cassino.Domain.Contracts;
using Cassino.Domain.Validation;
using FluentValidation.Results;

namespace Cassino.Domain.Entities;

public class Usuario : Entity, ISoftDelete, IAggregateRoot
{
    public string Nome { get; set; } = null!;
    public string? NomeSocial { get; set; }
    public string Email { get; set; } = null!;
    public string Cpf { get; set; } = null!;
    public string? Telefone { get; set; }
    public string Senha { get; set; } = null!;
    public DateOnly DataDeNascimento { get; set; }
    public bool Desativado { get; set; }
    public decimal Saldo { get; set; }
    public string? CodigoRecuperacaoSenha { get; set; }
    public DateTime? TempoExpiracaoDoCodigo { get; set; }
    public virtual List<Aposta> Apostas { get; set; } = null!;
    public virtual List<Pagamento> Pagamentos { get; set; } = null!;
    public virtual List<Saque> Saques { get; set; } = null!;

    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new UsuarioValidator().Validate(this);
        return validationResult.IsValid;
    }
}