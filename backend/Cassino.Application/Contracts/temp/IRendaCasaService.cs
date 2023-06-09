using Cassino.Application.Dtos.V1.Aposta;
using Cassino.Domain.Entities.temp;

namespace Cassino.Application.Contracts.temp;

public interface IRendaCasaService
{
    Task Adicionar(decimal valor);
    Task<bool> MovimentacaoRenda(AdicionarApostaDto apostaDto, Renda renda);
    Task<Renda> ObterCasa();
    Task<decimal> ObterRendaCasa();
    Task<decimal?> MudarRendaCasa(decimal valor);
}