using AutoMapper;
using Cassino.Application.Contracts;
using Cassino.Application.Dtos.V1.Saldo;
using Cassino.Application.Notification;
using Cassino.Domain.Contracts.Repositories;
using Cassino.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassino.Application.Services
{
    public class SaldoService : BaseService, ISaldoService
    {
        private readonly IUsuarioRepository _clienteRepository;

        public SaldoService(IMapper mapper, INotificator notificator, IUsuarioRepository clienteRepository) : base(mapper, notificator)
        {
            _clienteRepository = clienteRepository;
        }
        

        public async Task<SaldoUsuarioDto> BuscarSaldo(int id)
        {
            var usuario = await _clienteRepository.ObterPorId(id);
            if (usuario != null)
            {
                return Mapper.Map<SaldoUsuarioDto>(usuario);
            }

            Notificator.HandleNotFoundResource();
            return null;
        }

        public async Task<Boolean> AtualizarSaldo(SaldoUsuarioDto saldoUsuarioDto)
        {
            var usuario = await _clienteRepository.ObterPorId(saldoUsuarioDto.Id);
            if (usuario == null)
            {
                Notificator.Handle("Não foi possível atualizar o saldo do usuário no banco de dados.");
                return false;
            }

            usuario.Saldo = saldoUsuarioDto.Saldo;
            _clienteRepository.Alterar(usuario);

            if (await _clienteRepository.UnitOfWork.Commit())
                return true;
            return false;
        }
    }
}
