﻿using Cassino.Application.Contracts;
using Cassino.Application.Dtos.V1.Auth;
using Cassino.Application.Dtos.V1.Senha;
using Cassino.Application.Notification;
using Cassino.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using OpenTracing.Tag;
using Swashbuckle.AspNetCore.Annotations;

namespace Cassino.Api.Controllers.Usuario
{
    [Route("v{version:apiVersion}/Senha/[controller]")]
    public class UsuarioSenhaController : BaseController
    {
        public readonly ISenhaService _senhaService;
        public UsuarioSenhaController(INotificator notificator, ISenhaService senhaService) : base(notificator)
        {
            _senhaService = senhaService;
        }

        [HttpPost("redefinir-senha")]
        [SwaggerOperation(Summary = "Envia um e-mail de redefinição de senha para o usuario deslogado.", Tags = new[] { "Usuario - Cliente - Senha" })]
        [ProducesResponseType(typeof(Nullable), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Nullable), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RedefinirSenha([FromForm] string email)
        {
            var usuario = await _senhaService.EmailExiste(email);
            if (usuario == null)
                return BadRequest();
            string? link = await _senhaService.GerarLinkRedefinicaoSenha(usuario);
            if (link == null)
                return BadRequest();
            var EmailFoiEnviado = _senhaService.EmailRedefinicaoSenha(email, link);
            if (EmailFoiEnviado)
                return NoContentResponse();
            return BadRequest();
        }

        [HttpPost("alterar-senha-deslogado/codigo={code}")]
        [SwaggerOperation(Summary = "Verifica e salva uma nova senha para o usuário deslogado.", Tags = new[] { "Usuario - Cliente - Senha" })]
        [ProducesResponseType(typeof(Nullable), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Nullable), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AlterarSenhaDeslogado(string code, [FromForm] AlterarSenhaDeslogadoDto novaSenha) 
        {
            var usuario = await _senhaService.CodigoExiste(code);
            if (usuario == null)
                return BadRequest();
            if (!_senhaService.VerificarSenha(novaSenha))
                return BadRequest();
            if (await _senhaService.SalvarNovaSenha(usuario, novaSenha))
                return NoContentResponse();
            return BadRequest();
        }
    }
}
