using System;
using System.Threading.Tasks;
using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Constants;
using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContaCorrente.Api.Controllers
{
    [ApiController]
    [Route("api/contas/{id}/movimentos")]
    [Authorize]
    [Produces("application/json")]
    public class MovimentosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MovimentosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lança um novo movimento na conta
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <param name="request">Dados do movimento</param>
        /// <returns>Dados do movimento criado e saldo atual</returns>
        /// <response code="201">Movimento criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Conta não encontrada</response>
        /// <response code="409">Saldo insuficiente</response>
        [HttpPost]
        [ProducesResponseType(typeof(LancarMovimentoResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<ActionResult<LancarMovimentoResponse>> LancarMovimento(
            string id,
            [FromBody] LancarMovimentoRequest request
        )
        {
            try
            {
                // Obter chave de idempotência do header
                var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();

                var command = new LancarMovimentoCommand(
                    id,
                    request.Data,
                    request.Tipo,
                    request.Valor
                );
                var result = await _mediator.Send(command);

                return CreatedAtAction(
                    nameof(ObterMovimento),
                    new { id, movimentoId = result.IdMovimento },
                    result
                );
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Saldo insuficiente"))
            {
                return Conflict(
                    new ErrorResponse { Error = ex.Message, Code = ErrorCodes.SALDO_INSUFICIENTE }
                );
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Conta inativa"))
            {
                return BadRequest(
                    new ErrorResponse { Error = ex.Message, Code = ErrorCodes.INACTIVE_ACCOUNT }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new ErrorResponse { Error = ex.Message, Code = ErrorCodes.DADOS_INVALIDOS }
                );
            }
        }

        /// <summary>
        /// Obtém um movimento específico
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <param name="movimentoId">ID do movimento</param>
        /// <returns>Dados do movimento</returns>
        /// <response code="200">Movimento encontrado</response>
        /// <response code="404">Movimento não encontrado</response>
        [HttpGet("{movimentoId}")]
        [ProducesResponseType(typeof(LancarMovimentoResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<ActionResult<LancarMovimentoResponse>> ObterMovimento(
            string id,
            string movimentoId
        )
        {
            // Implementar query para obter movimento por ID
            return NotFound(
                new ErrorResponse
                {
                    Error = "Movimento não encontrado",
                    Code = "MOVIMENTO_NAO_ENCONTRADO",
                }
            );
        }
    }
}
