using Cassino.Application.Contracts;
using Cassino.Application.Dtos.V1.Pagamentos;
using Cassino.Application.Hubs;
using Cassino.Application.Notification;
using Cassino.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RestSharp;
using Swashbuckle.AspNetCore.Annotations;

namespace Cassino.Api.Controllers.Usuario;

[Route("v{version:apiVersion}/Cliente/[controller]")]
public class UsuarioCarteiraController : BaseController
{
    private readonly IHubContext<PixHub> _hubContext;
    private readonly IUsuarioCarteiraService _service;
    public UsuarioCarteiraController(INotificator notificator, IUsuarioCarteiraService service, IHubContext<PixHub> hubContext) : base(notificator)
    {
        _service = service;
        _hubContext = hubContext;
    }

    [AllowAnonymous]
    [HttpPost]
    [SwaggerOperation(Summary = "Realizar depósito pix de um Cliente.", Tags = new [] { "Usuario - Carteira" })]
    [ProducesResponseType(typeof(PixDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> PagarComPix([FromBody] DadosPagamentoPixDto dto)
    {
        var pix = await  _service.Deposito(dto);
        return OkResponse(pix);
    }
    
    // [AllowAnonymous]
    // [HttpPost("/autenticar")]
    // [SwaggerOperation(Summary = "Autenticação.", Tags = new [] { "Usuario - Carteira" })]
    // [ProducesResponseType(typeof(PixDto), StatusCodes.Status201Created)]
    // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    // [ProducesResponseType(StatusCodes.Status403Forbidden)]
    // public async Task<IActionResult> Autenticar([FromBody] DadosPagamentoPixDto dto)
    // {
    //     var pix = await  _service.Autenticar();
    //     return OkResponse(pix);
    // }
    
    [AllowAnonymous]
    [HttpPost("/testando")]
    public async Task<IActionResult> EnviarNotificacao([FromBody] string mensagem)
    {
        await _hubContext.Clients.All.SendAsync("ReceberNotificacao", mensagem);
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet]
    [SwaggerOperation(Summary = "Ver últimos depósitos pix de um Cliente.", Tags = new [] { "Usuario - Carteira" })]
    public async Task<IActionResult> UltimosPagamentos()
    {
        var client = new RestClient("https://api.pagar.me/core/v5/orders");
        var request = new RestRequest();
        request.Method = Method.Get;
        request.AddHeader("accept", "application/json");
        request.AddHeader("content-type", "application/json");
        request.AddHeader("authorization", "Basic c2tfTlE5V3FNa1RRaWFiSk0ycjo=");
        RestResponse response = await client.ExecuteAsync(request);
        return Ok(response.Content);
    }

    [AllowAnonymous]
    [HttpPost("webhook/")]
    [SwaggerOperation(Summary = "Webhook confirmação pix.", Tags = new[] { "Usuario - Carteira" })]
    [ProducesResponseType(typeof(Pagamento), StatusCodes.Status201Created)]
    public async Task<IActionResult> PixWebHook()
    {
        var retorno = Request;
        var pagamento = await _service.WebhookPix(retorno);
        return OkResponse(pagamento);
    }
    
    [HttpPost("/saque")]
    public async Task<IActionResult> Saque()
    {

        return Ok();
    }
}