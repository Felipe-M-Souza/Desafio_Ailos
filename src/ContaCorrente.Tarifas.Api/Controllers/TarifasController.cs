using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Constants;
using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ContaCorrente.Tarifas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class TarifasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TarifasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria uma nova tarifa
        /// </summary>
        /// <param name="request">Dados da tarifa</param>
        /// <returns>Dados da tarifa criada</returns>
        /// <response code="201">Tarifa criada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="409">Tarifa já existe</response>
        [HttpPost]
        [ProducesResponseType(typeof(TarifaResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<ActionResult<TarifaResponse>> CriarTarifa([FromBody] CriarTarifaRequest request)
        {
            try
            {
                var command = new CriarTarifaCommand(request.TipoOperacao, request.Valor, request.Descricao);
                var result = await _mediator.Send(command);

                return CreatedAtAction(nameof(ObterTarifa), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.OPERACAO_INVALIDA });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.DADOS_INVALIDOS });
            }
        }

        /// <summary>
        /// Obtém todas as tarifas
        /// </summary>
        /// <returns>Lista de tarifas</returns>
        /// <response code="200">Tarifas obtidas com sucesso</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TarifaResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public async Task<ActionResult<IEnumerable<TarifaResponse>>> ObterTarifas()
        {
            try
            {
                var query = new ObterTarifasQuery();
                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.DADOS_INVALIDOS });
            }
        }

        /// <summary>
        /// Obtém uma tarifa específica
        /// </summary>
        /// <param name="id">ID da tarifa</param>
        /// <returns>Dados da tarifa</returns>
        /// <response code="200">Tarifa encontrada</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Tarifa não encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TarifaResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<ActionResult<TarifaResponse>> ObterTarifa(string id)
        {
            try
            {
                var query = new ObterTarifaQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ErrorResponse { Error = "Tarifa não encontrada", Code = "TARIFA_NAO_ENCONTRADA" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.DADOS_INVALIDOS });
            }
        }

        /// <summary>
        /// Obtém tarifas cobradas de uma conta
        /// </summary>
        /// <param name="idConta">ID da conta</param>
        /// <param name="dataInicio">Data de início (dd/MM/yyyy)</param>
        /// <param name="dataFim">Data de fim (dd/MM/yyyy)</param>
        /// <returns>Lista de tarifas cobradas</returns>
        /// <response code="200">Tarifas cobradas obtidas com sucesso</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet("cobradas/{idConta}")]
        [ProducesResponseType(typeof(IEnumerable<TarifaCobradaResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public async Task<ActionResult<IEnumerable<TarifaCobradaResponse>>> ObterTarifasCobradas(
            string idConta,
            [FromQuery] string? dataInicio = null,
            [FromQuery] string? dataFim = null)
        {
            try
            {
                var query = new ObterTarifasCobradasQuery(idConta, dataInicio, dataFim);
                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.DADOS_INVALIDOS });
            }
        }

        /// <summary>
        /// Ativa ou desativa uma tarifa
        /// </summary>
        /// <param name="id">ID da tarifa</param>
        /// <param name="request">Dados para ativação/desativação</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="200">Tarifa atualizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Tarifa não encontrada</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(TarifaResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<ActionResult<TarifaResponse>> AtualizarStatusTarifa(
            string id, 
            [FromBody] AtualizarStatusTarifaRequest request)
        {
            try
            {
                // TODO: Implementar comando para atualizar status da tarifa
                return BadRequest(new ErrorResponse { Error = "Funcionalidade em desenvolvimento", Code = ErrorCodes.OPERACAO_INVALIDA });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ErrorResponse { Error = ex.Message, Code = "TARIFA_NAO_ENCONTRADA" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Error = ex.Message, Code = ErrorCodes.DADOS_INVALIDOS });
            }
        }
    }
}

