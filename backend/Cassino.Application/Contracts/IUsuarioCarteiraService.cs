using Cassino.Application.Dtos.V1.Pagamentos;

namespace Cassino.Application.Contracts;

public interface IUsuarioCarteiraService
{
    Task<PixDto?> Deposito(DadosPagamentoPixDto dto);
}