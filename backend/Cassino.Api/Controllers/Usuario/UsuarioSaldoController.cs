using Cassino.Application.Contracts;
using Cassino.Application.Dtos.V1.Saldo;
using Cassino.Application.Notification;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cassino.Api.Controllers.Usuario;

[Route("v{version:apiVersion}/Saldo/[controller]")]
public class UsuarioSaldoController : BaseController
{
    private readonly ISaldoService _saldoService;
    public UsuarioSaldoController(INotificator notificator, ISaldoService saldoService) : base(notificator)
    {
        _saldoService = saldoService;
    }

    [HttpGet("buscar-saldo/{id}")]
    [SwaggerOperation(Summary = "Consulta Saldo de um Cliente.", Tags = new[] { "Usuario - Cliente - Saldo" })]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Nullable), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarSaldo(int id)
    {
        var saldo = await _saldoService.BuscarSaldo(id);
        return Ok(saldo.Saldo);
    }

    [HttpPut]
    [SwaggerOperation(Summary = "Atualiza o Saldo de um Cliente.", Tags = new[] { "Usuario - Cliente - Saldo" })]
    [ProducesResponseType(typeof(Nullable), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Nullable), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AtualizarSaldo([FromForm] SaldoUsuarioDto saldoUsuarioDto)
    {
        var usuarioAtualizado = await _saldoService.AtualizarSaldo(saldoUsuarioDto);
        return usuarioAtualizado ? NoContentResponse() : BadRequest();
    }
}