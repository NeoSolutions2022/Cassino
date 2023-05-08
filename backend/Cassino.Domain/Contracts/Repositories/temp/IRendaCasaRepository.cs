using Cassino.Domain.Entities.temp;

namespace Cassino.Domain.Contracts.Repositories.temp;

public interface IRendaCasaRepository : IDisposable
{
    
    IUnitOfWork UnitOfWork { get; }
    void Adicionar(Renda renda);
}