﻿using AutoMapper;
using Cassino.Application.Contracts;
using Cassino.Application.Dtos.V1.Auth;
using Cassino.Application.Dtos.V1.Senha;
using Cassino.Application.Notification;
using Cassino.Core.Settings;
using Cassino.Domain.Contracts.Repositories;
using Cassino.Domain.Entities;
using Cassino.Infra.Repositories;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using RazorLight;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Net.WebRequestMethods;

namespace Cassino.Application.Services
{
    public class SenhaService : BaseService, ISenhaService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        public SenhaService(IUsuarioRepository usuarioRepository, IPasswordHasher<Usuario> passwordHasher, INotificator notificator, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration config) : base(mapper, notificator)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }


        //Metodos de SolicitarRedefinicaoSenha
        public async Task<Usuario> EmailExiste(string email)
        {
            var usuario = await _usuarioRepository.ObterPorEmail(email);
            if (usuario != null)
                 return usuario;
            Notificator.HandleNotFoundResource();
            return null;
        }


        public async Task<Usuario?> GerarCodigoRedefinicaoSenha(Usuario usuario)
        {
            Guid guid = Guid.NewGuid();
            string codigo = guid.ToString();
            DateTime tempoExpiracaoCodigo = DateTime.UtcNow.AddHours(3);

            //Salva o codigo e o timeStamp no banco atrelado a conta do usuario. 
            usuario.CodigoRecuperacaoSenha = codigo;
            usuario.TempoExpiracaoDoCodigo = tempoExpiracaoCodigo;
            _usuarioRepository.Alterar(usuario);
            if(!await _usuarioRepository.UnitOfWork.Commit())
            {
                Notificator.Handle("Ocorreu um problema ao salvar Codigo de Recuperação no banco de dados.");
                return null;
            }

            return usuario;
        }


        public async Task<bool> EmailRedefinicaoSenha(Usuario usuarioPreenchido)
        {
            //Configuração modelo template e-mail
            string baseDirectoryPath = @"Cassino\backend\";
            
            var engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(baseDirectoryPath)
                .UseMemoryCachingProvider()
                .Build();

            var modeloEmail = new ModeloEmailDto
            {
                Nome = usuarioPreenchido.Nome,
                Codigo = usuarioPreenchido.CodigoRecuperacaoSenha,
                Url = "https://sys-bet.vercel.app",
                ExpiracaoEmHoras = 3
            };

            string template = await engine.CompileRenderAsync("TemplateEmailResetarSenha.cshtml", modeloEmail);

            //Configuração E-mail
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailRemetenteUsername").Value));
            email.To.Add(MailboxAddress.Parse(usuarioPreenchido.Email));
            email.Subject = "Redefinição de Senha";
            email.Body = new TextPart(TextFormat.Html) { Text = template };

            //Configuração de servidor SMTP Gmail
            using var smtp = new SmtpClient();
            try
            {
                smtp.Connect(_config.GetSection("EmailProvedor").Value, int.Parse(_config.GetSection("EmailPort").Value), SecureSocketOptions.StartTls);
                smtp.Authenticate(_config.GetSection("EmailRemetenteUsername").Value, _config.GetSection("EmailRemetenteSenha").Value);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                Notificator.Handle("Ocorreu um problema ao tentar enviar o e-mail de redefinição de senha. " + ex.Message);
                return false;
            }
            return true;
        }

        public async Task<bool> Solicitar(string email)
        {
            var usuario = await EmailExiste(email);
            if (usuario == null)
                return false;
            var usuarioPreenchido = await GerarCodigoRedefinicaoSenha(usuario);
            if (usuarioPreenchido == null)
                return false;
            var EmailFoiEnviado = await EmailRedefinicaoSenha(usuarioPreenchido);
            if (EmailFoiEnviado)
                return true;
            return false;
        }


        //Metodos de RedefinirSenha
        public async Task<bool> Redefinir(string codigo, AlterarSenhaDto novaSenha)
        {
            var usuario = await CodigoExiste(codigo);
            if (usuario == null)
                return false;
            if (!VerificarSenha(novaSenha))
                return false;
            if (await SalvarNovaSenha(usuario, novaSenha))
                return true;
            return false;
        }

        public async Task<Usuario?> CodigoExiste(string codigo)
        {
            var usuario = await _usuarioRepository.ObterPorCodigoRecuperacaoSenha(codigo);
            if(usuario != null)
            {
                if(DateTime.UtcNow > usuario.TempoExpiracaoDoCodigo)
                {
                    //Apagando codigo e timeStamp do usuario.
                    Notificator.Handle("O tempo do código de redefinição expirou.");

                    usuario.CodigoRecuperacaoSenha = null;
                    usuario.TempoExpiracaoDoCodigo = null;

                    _usuarioRepository.Alterar(usuario);
                    await _usuarioRepository.UnitOfWork.Commit();
                    return null;
                }

                return usuario;
            }
            
            Notificator.HandleNotFoundResource();
            return null;
        }


        public bool VerificarSenha(AlterarSenhaDto novaSenha)
        {
            if (novaSenha.NovaSenha == novaSenha.ConfirmarNovaSenha)
                return true;
            Notificator.Handle("Senha e confirmação de senha não são iguais.");
            return false;
        }


        public async Task<bool> SalvarNovaSenha(Usuario usuario, AlterarSenhaDto alterarSenha)
        {
            usuario.CodigoRecuperacaoSenha = null;
            usuario.TempoExpiracaoDoCodigo = null;

            usuario.Senha = alterarSenha.NovaSenha;
            usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
            _usuarioRepository.Alterar(usuario);
            if (await _usuarioRepository.UnitOfWork.Commit())
            {
                return true;
            }
            Notificator.Handle("Ocorreu um problema ao salvar nova senha no banco.");
            return false;
        }


        public async Task<bool> AlterarSenhaLogin(string senhaAntiga, AlterarSenhaDto alterarSenhaDto)
        {
            //Buscando usuario por meio do HTTPContext
            var usuarioId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if(usuarioId == null)
                return false;
            var usuario = await _usuarioRepository.ObterPorId(Int32.Parse(usuarioId));

            //Comparando senha antiga com a passada.
            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senhaAntiga);
            if (resultado == PasswordVerificationResult.Failed)
            {
                Notificator.Handle("Senha incorreta.");
                return false;
            }

            //Comparando as duas novas senhas (senha e confirmar senha)
            if (alterarSenhaDto.NovaSenha != alterarSenhaDto.ConfirmarNovaSenha)
            {
                Notificator.Handle("Senha e confirmação de senha não são iguais.");
                return false;
            }
            
            usuario.Senha = _passwordHasher.HashPassword(usuario, alterarSenhaDto.NovaSenha);
            _usuarioRepository.Alterar(usuario);
            if (!await _usuarioRepository.UnitOfWork.Commit())
            {
                Notificator.Handle("Ocorreu um problema ao salvar nova senha no banco.");
                return false;
            }
            return true;
        }
    }
}
