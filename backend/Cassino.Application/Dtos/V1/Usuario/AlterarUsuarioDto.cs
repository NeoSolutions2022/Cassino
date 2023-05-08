namespace Cassino.Application.Dtos.V1.Usuario;

public class AlterarUsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? NomeSocial { get; set; }
    public string Email { get; set; } = null!;
    public string Cpf { get; set; } = null!;
    public string? Telefone { get; set; }
    public DateOnly DataDeNascimento { get; set; }
    public bool Desativado { get; set; }
}