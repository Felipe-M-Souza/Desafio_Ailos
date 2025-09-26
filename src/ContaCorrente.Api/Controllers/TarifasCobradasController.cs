using System;
using System.Threading.Tasks;
using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContaCorrente.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class TarifasCobradasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TarifasCobradasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtém tarifas cobradas de uma conta
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <param name="dataInicio">Data de início (DD/MM/YYYY)</param>
        /// <param name="dataFim">Data de fim (DD/MM/YYYY)</param>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Tarifas cobradas</returns>
        /// <response code="200">Tarifas cobradas retornadas com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Conta não encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TarifaCobradaResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<ActionResult<TarifaCobradaResponse>> ObterTarifasCobradas(
            string id,
            [FromQuery] string? dataInicio = null,
            [FromQuery] string? dataFim = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50
        )
        {
            try
            {
                var query = new ObterTarifasCobradasQuery(id, dataInicio, dataFim);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new ErrorResponse { Error = ex.Message, Code = "DADOS_INVALIDOS" }
                );
            }
        }
    }
}
