using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Constants;
using ContaCorrente.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ContaCorrente.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
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
        /// <param name="id">ID da conta origem</param>
        /// <param name="request">Dados da transferência</param>
        /// <returns>Resultado da transferência</returns>
        /// <response code="200">Transferência realizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Conta não encontrada</response>
        /// <response code="409">Saldo insuficiente ou conta inativa</response>
        [HttpPost("{id}/transferir")]
        [ProducesResponseType(typeof(TransferirEntreContasResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<ActionResult<TransferirEntreContasResponse>> TransferirEntreContas(
            string id, 
            [FromBody] TransferirEntreContasRequest request)
        {
            try
            {
                var command = new TransferirEntreContasCommand(
                    id, 
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
                return Conflict(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.OPERACAO_INVALIDA });
            }
        }
    }
}
