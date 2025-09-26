using ContaCorrente.Application.Commands;
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
    [Authorize]
    public class TarifasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TarifasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtém todas as tarifas configuradas
        /// </summary>
        /// <returns>Lista de tarifas</returns>
        /// <response code="200">Lista de tarifas retornada com sucesso</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TarifaResponse>), 200)]
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
                return BadRequest(new ErrorResponse { Error = ex.Message, Code = "ERRO_INTERNO" });
            }
        }

        /// <summary>
        /// Cria uma nova tarifa
        /// </summary>
        /// <param name="request">Dados da tarifa</param>
        /// <returns>Tarifa criada</returns>
        /// <response code="200">Tarifa criada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="409">Tarifa já existe para este tipo de operação</response>
        [HttpPost]
        [ProducesResponseType(typeof(TarifaResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<ActionResult<TarifaResponse>> CriarTarifa([FromBody] CriarTarifaRequest request)
        {
            try
            {
                var command = new CriarTarifaCommand(request.TipoOperacao, request.Valor, request.Descricao);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse { Error = ex.Message, Code = "DADOS_INVALIDOS" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ErrorResponse { Error = ex.Message, Code = "TARIFA_EXISTENTE" });
            }
        }
    }
}
