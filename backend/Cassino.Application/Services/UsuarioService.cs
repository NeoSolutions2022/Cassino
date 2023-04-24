using AutoMapper;
using Cassino.Application.Contracts;
using Cassino.Application.Dtos.V1.Base;
using Cassino.Application.Dtos.V1.Saldo;
using Cassino.Application.Dtos.V1.Usuario;
using Cassino.Application.Notification;
using Cassino.Domain.Contracts.Repositories;
using Cassino.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Cassino.Application.Services;

public class UsuarioService : BaseService, IUsuarioService
{
    private readonly IUsuarioRepository _clienteRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UsuarioService(IMapper mapper, INotificator notificator, IUsuarioRepository clienteRepository, IPasswordHasher<Usuario> passwordHasher) : base(mapper, notificator)
    {
        _clienteRepository = clienteRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<PagedDto<UsuarioDto>> Buscar(BuscarUsuarioDto dto)
    {
        var usuario = await _clienteRepository.Buscar(dto);
        return Mapper.Map<PagedDto<UsuarioDto>>(usuario);
    }

    public async Task<UsuarioDto?> Cadastrar(CadastrarUsuarioDto dto)
    {
        var usuario = Mapper.Map<Usuario>(dto);
        if (!await Validar(usuario))
        {
            return null;
        }
        
        usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
        _clienteRepository.Adicionar(usuario);
        if (await _clienteRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }
        
        Notificator.Handle("Não foi possível adicionar o cliente");
        return null;
    }

    public async Task<UsuarioDto?> Alterar(int id, AlterarUsuarioDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem!");
            return null;
        }

        var usuario = await _clienteRepository.ObterPorId(id);
        if (usuario == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(usuario, dto);
        if (!await Validar(usuario))
        {
            return null;
        }
        
        usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
        _clienteRepository.Alterar(usuario);
        if (await _clienteRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }
        
        Notificator.Handle("Não foi possível alterar o cliente");
        return null;
    }

    public async Task<UsuarioDto?> ObterPorId(int id)
    {
        var usuario = await _clienteRepository.ObterPorId(id);
        if (usuario != null)
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }
        
        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<UsuarioDto?> ObterPorEmail(string email)
    {
        var usuario = await _clienteRepository.ObterPorEmail(email);
        if (usuario != null)
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }
        
        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<UsuarioDto?> ObterPorCpf(string cpf)
    {
        var usuario = await _clienteRepository.ObterPorCpf(cpf);
        if (usuario != null)
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }
        
        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task Desativar(int id)
    {
        var usuario = await _clienteRepository.ObterPorId(id);
        if (usuario == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        usuario.Desativado = true;
        _clienteRepository.Alterar(usuario);
        if (await _clienteRepository.UnitOfWork.Commit())
        {
            return;
        }
        
        Notificator.Handle("Não foi possível desativar o cliente");
    }

    public async Task Reativar(int id)
    {
        var usuario = await _clienteRepository.ObterPorId(id);
        if (usuario == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        usuario.Desativado = false;
        _clienteRepository.Alterar(usuario);
        if (await _clienteRepository.UnitOfWork.Commit())
        {
            return;
        }
        
        Notificator.Handle("Não foi possível reativar o cliente");
    }

    private async Task<bool> Validar(Usuario usuario)
    {
        if (!usuario.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }
        
        var usuarioExistente = await _clienteRepository.FistOrDefault(c =>
            c.Cpf == usuario.Cpf || c.Email == usuario.Email && c.Id != usuario.Id);
        if (usuarioExistente != null)
        {
            Notificator.Handle("Já existe um usuário cadastrador com uma ou mais identificações");
        }
        
        return !Notificator.HasNotification;
    }
}