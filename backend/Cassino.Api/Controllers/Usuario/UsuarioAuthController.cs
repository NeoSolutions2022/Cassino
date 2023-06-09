using Cassino.Application.Contracts;
using Cassino.Application.Dtos.V1.Auth;
using Cassino.Application.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cassino.Api.Controllers.Usuario;

[AllowAnonymous]
[Route("v{version:apiVersion}/Cliente/[controller]")]
public class ClientesAuthController : BaseController
{
    private readonly IUsuarioAuthService _usuarioAuthService;

    public ClientesAuthController(INotificator notificator, IUsuarioAuthService usuarioAuthService) : base(notificator)
    {
        _usuarioAuthService = usuarioAuthService;
    }

    [HttpPost("Login-Cliente")]
    [SwaggerOperation(Summary = "Login - Cliente.", Tags = new[] { "Usuario - Cliente - Autenticação" })]
    [ProducesResponseType(typeof(UsuarioAutenticadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginCliente([FromBody] LoginDto loginCliente)
    {
        var token = await _usuarioAuthService.Login(loginCliente);
        return token != null ? OkResponse(token) : Unauthorized(new[] { "Usuário e/ou senha incorretos" });
    }
}