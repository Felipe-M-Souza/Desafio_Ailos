using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Constants;
using ContaCorrente.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ContaCorrente.Transferencias.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class TransferenciasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransferenciasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Realiza transferência entre contas
        /// </summary>
        /// <param name="request">Dados da transferência</param>
        /// <returns>Resultado da transferência</returns>
        /// <response code="200">Transferência realizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Conta não encontrada</response>
        /// <response code="409">Saldo insuficiente ou conta inativa</response>
        [HttpPost]
        [ProducesResponseType(typeof(TransferirEntreContasResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<ActionResult<TransferirEntreContasResponse>> Transferir(
            [FromBody] TransferirEntreContasRequest request)
        {
            try
            {
                // Obter chave de idempotência do header
                var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
                
                // Obter ID da conta origem do token JWT
                var contaOrigemId = User.FindFirst("conta_id")?.Value;
                if (string.IsNullOrEmpty(contaOrigemId))
                {
                    return Unauthorized(new ErrorResponse 
                    { 
                        Error = "Token inválido - conta não identificada", 
                        Code = ErrorCodes.USER_UNAUTHORIZED 
                    });
                }

                var command = new TransferirEntreContasCommand(
                    contaOrigemId,
                    request.NumeroContaDestino,
                    request.Valor,
                    request.Data,
                    request.Descricao);
                
                var result = await _mediator.Send(command);
                
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.DADOS_INVALIDOS });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("Saldo insuficiente"))
                {
                    return Conflict(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.SALDO_INSUFICIENTE });
                }
                if (ex.Message.Contains("Conta inativa"))
                {
                    return Conflict(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.INACTIVE_ACCOUNT });
                }
                return Conflict(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.OPERACAO_INVALIDA });
            }
        }

        /// <summary>
        /// Obtém histórico de transferências da conta
        /// </summary>
        /// <param name="dataInicio">Data de início (dd/MM/yyyy)</param>
        /// <param name="dataFim">Data de fim (dd/MM/yyyy)</param>
        /// <returns>Lista de transferências</returns>
        /// <response code="200">Histórico obtido com sucesso</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet("historico")]
        [ProducesResponseType(typeof(IEnumerable<TransferenciaHistoricoResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public async Task<ActionResult<IEnumerable<TransferenciaHistoricoResponse>>> ObterHistorico(
            [FromQuery] string? dataInicio = null,
            [FromQuery] string? dataFim = null)
        {
            try
            {
                // Obter ID da conta do token JWT
                var contaId = User.FindFirst("conta_id")?.Value;
                if (string.IsNullOrEmpty(contaId))
                {
                    return Unauthorized(new ErrorResponse 
                    { 
                        Error = "Token inválido - conta não identificada", 
                        Code = ErrorCodes.USER_UNAUTHORIZED 
                    });
                }

                // TODO: Implementar query para obter histórico de transferências
                // Por enquanto, retornar lista vazia
                return Ok(new List<TransferenciaHistoricoResponse>());
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.DADOS_INVALIDOS });
            }
        }
    }
}
