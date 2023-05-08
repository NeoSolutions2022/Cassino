using AutoMapper;
using Cassino.Application.Contracts.temp;
using Cassino.Application.Notification;
using Cassino.Domain.Contracts.Repositories.temp;
using Cassino.Domain.Entities.temp;

namespace Cassino.Application.Services.temp;

public class RendaCasaService : BaseService, IRendaCasaService
{
    private readonly IRendaCasaRepository _rendaCasaRepository;
    public RendaCasaService(IMapper mapper, INotificator notificator, IRendaCasaRepository rendaCasaRepository) : base(mapper, notificator)
    {
        _rendaCasaRepository = rendaCasaRepository;
    }

    public async Task Adicionar(decimal valor)
    {
        var renda = new Renda();
        renda.Valor = valor;
        _rendaCasaRepository.Adicionar(renda);
        if (!await _rendaCasaRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Deu errado amigo!");
        }
    }
}