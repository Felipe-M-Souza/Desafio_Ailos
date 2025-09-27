using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transferencias.Application.Handlers;

namespace Transferencias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransferenciasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransferenciasController> _logger;

    public TransferenciasController(IMediator mediator, ILogger<TransferenciasController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Realiza transferência entre contas da mesma instituição
    /// </summary>
    /// <param name="request">Dados da transferência</param>
    /// <returns>Resultado da transferência</returns>
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Transferir([FromBody] TransferirRequest request)
    {
        try
        {
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { Mensagem = "Token de autorização não fornecido", TipoErro = "UNAUTHORIZED" });
            }

            var command = new TransferirEntreContasCommand
            {
                NumeroContaOrigem = request.NumeroContaOrigem,
                NumeroContaDestino = request.NumeroContaDestino,
                Valor = request.Valor,
                Descricao = request.Descricao,
                Token = token
            };

            var result = await _mediator.Send(command);

            if (result.Sucesso)
            {
                return NoContent();
            }

            return BadRequest(new { Mensagem = result.Erro, TipoErro = result.TipoErro });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar transferência");
            return StatusCode(500, new { Mensagem = "Erro interno do servidor", TipoErro = "INTERNAL_ERROR" });
        }
    }
}

public class TransferirRequest
{
    /// <summary>
    /// Número da conta de origem
    /// </summary>
    public int NumeroContaOrigem { get; set; }

    /// <summary>
    /// Número da conta de destino
    /// </summary>
    public int NumeroContaDestino { get; set; }

    /// <summary>
    /// Valor a ser transferido
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Descrição da transferência
    /// </summary>
    public string Descricao { get; set; } = string.Empty;
}
