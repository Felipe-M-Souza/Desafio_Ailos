using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Constants;
using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Queries;
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
    public class ContasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria uma nova conta corrente
        /// </summary>
        /// <param name="request">Dados da conta a ser criada</param>
        /// <returns>Dados da conta criada</returns>
        /// <response code="201">Conta criada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="409">Número de conta já existe</response>
        [HttpPost]
        [ProducesResponseType(typeof(ContaResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<ActionResult<ContaResponse>> CriarConta([FromBody] CriarContaRequest request)
        {
            try
            {
                var command = new CriarContaCommand(request.Numero, request.Nome, request.Cpf, request.Senha);
                var result = await _mediator.Send(command);
                
                return CreatedAtAction(nameof(ObterConta), new { id = result.Id }, result);
            }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.CONTA_EXISTENTE });
        }
        catch (ArgumentException ex)
        {
            var errorCode = ex.ParamName == "Cpf" ? ErrorCodes.INVALID_DOCUMENT : ErrorCodes.DADOS_INVALIDOS;
            return BadRequest(new ErrorResponse { Error = ex.Message, Code = errorCode });
        }
        }

        /// <summary>
        /// Ativa ou desativa uma conta corrente
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <param name="request">Status de ativação</param>
        /// <returns>Dados da conta atualizada</returns>
        /// <response code="200">Conta atualizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Conta não encontrada</response>
        [HttpPatch("{id}/ativar")]
        [ProducesResponseType(typeof(ContaResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<ActionResult<ContaResponse>> AtivarConta(string id, [FromBody] AtivarContaRequest request)
        {
            try
            {
                var command = new AtivarContaCommand(id, request.Ativo);
                var result = await _mediator.Send(command);
                
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ErrorResponse { Error = ex.Message, Code = "CONTA_NAO_ENCONTRADA" });
            }
        }

        /// <summary>
        /// Inativa uma conta corrente
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <param name="request">Dados para inativação</param>
        /// <returns>Resultado da inativação</returns>
        /// <response code="204">Conta inativada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Senha inválida</response>
        /// <response code="404">Conta não encontrada</response>
        /// <response code="409">Conta já inativa</response>
        [HttpPatch("{id}/inativar")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<ActionResult> InativarConta(string id, [FromBody] InativarContaRequest request)
        {
            try
            {
                var command = new InativarContaCommand(id, request.Senha);
                await _mediator.Send(command);
                
                return NoContent();
            }
        catch (ArgumentException ex)
        {
            return NotFound(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.CONTA_NAO_ENCONTRADA });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.USER_UNAUTHORIZED });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.CONTA_JA_INATIVA });
        }
        }

        /// <summary>
        /// Obtém dados de uma conta corrente
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <returns>Dados da conta</returns>
        /// <response code="200">Conta encontrada</response>
        /// <response code="404">Conta não encontrada</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ContaResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<ActionResult<ContaResponse>> ObterConta(string id)
        {
            try
            {
                var query = new ObterContaQuery(id);
                var result = await _mediator.Send(query);
                
                if (result == null)
                {
                    return NotFound(new ErrorResponse { Error = "Conta não encontrada", Code = "CONTA_NAO_ENCONTRADA" });
                }
                
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ErrorResponse { Error = ex.Message, Code = "CONTA_NAO_ENCONTRADA" });
            }
        }
    }

    public class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
