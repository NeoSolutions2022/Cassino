namespace Cassino.Application.Dtos.V1.Usuario;

public class CadastrarUsuarioDto
{
    public string Nome { get; set; } = null!;
    public string? NomeSocial { get; set; }
    public string Email { get; set; } = null!;
    public string Cpf { get; set; } = null!;
    public string? Telefone { get; set; }
    public string Senha { get; set; } = null!;
    public DateOnly DataDeNascimento { get; set; }
    public bool Desativado { get; set; }
    public string ChavePix { get; set; } = null!;
}