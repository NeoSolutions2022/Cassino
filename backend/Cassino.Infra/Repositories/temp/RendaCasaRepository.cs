using Cassino.Domain.Contracts;
using Cassino.Domain.Contracts.Repositories.temp;
using Cassino.Domain.Entities.temp;
using Cassino.Infra.Context;

namespace Cassino.Infra.Repositories.temp;

public class RendaCasaRepository : IRendaCasaRepository
{
    private bool _isDisposed;
    protected readonly BaseApplicationDbContext Context;
    public RendaCasaRepository(BaseApplicationDbContext context)
    {
        Context = context;
    }
    
    public IUnitOfWork UnitOfWork => Context;
    public void Adicionar(Renda renda)
    {
        Context.Rendas.Add(renda);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            Context.Dispose();
        }

        _isDisposed = true;
    }
    
    ~RendaCasaRepository()
    {
        Dispose(false);
    }
}