using Cassino.Application.Dtos.V1.Base;
using Cassino.Application.Dtos.V1.Usuario;
using Cassino.Application.Dtos.V1.Saldo;

namespace Cassino.Application.Contracts;

public interface IUsuarioService
{
    Task<PagedDto<UsuarioDto>> Buscar(BuscarUsuarioDto dto);
    Task<UsuarioDto?> Cadastrar(CadastrarUsuarioDto dto);
    Task<UsuarioDto?> Alterar(int id, AlterarUsuarioDto dto);
    Task<UsuarioDto?> ObterPorId(int id);
    Task<UsuarioDto?> ObterPorEmail(string email);
    Task<UsuarioDto?> ObterPorCpf(string cpf);
    Task Desativar(int id);
    Task Reativar(int id);
}