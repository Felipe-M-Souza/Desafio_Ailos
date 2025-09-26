using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace ContaCorrente.Api.Controllers
{
    [ApiController]
    [Route("api/contas/{id}/extrato")]
    [Authorize]
    [Produces("application/json")]
    public class ExtratoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;

        public ExtratoController(IMediator mediator, IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        /// <summary>
        /// Obtém o extrato da conta com filtros opcionais
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <param name="dataInicio">Data de início (DD/MM/YYYY)</param>
        /// <param name="dataFim">Data de fim (DD/MM/YYYY)</param>
        /// <param name="page">Página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 50)</param>
        /// <returns>Extrato da conta</returns>
        /// <response code="200">Extrato obtido com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Conta não encontrada</response>
        [HttpGet]
        [ProducesResponseType(typeof(ExtratoResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<ActionResult<ExtratoResponse>> ObterExtrato(
            string id,
            [FromQuery] string? dataInicio = null,
            [FromQuery] string? dataFim = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                // Verificar cache
                var cacheKey = $"extrato_{id}_{dataInicio}_{dataFim}_{page}_{pageSize}";
                if (_cache.TryGetValue(cacheKey, out ExtratoResponse? cachedExtrato))
                {
                    return Ok(cachedExtrato);
                }

                var query = new ObterExtratoQuery(id, dataInicio, dataFim, page, pageSize);
                var result = await _mediator.Send(query);

                // Cache por 10 segundos
                _cache.Set(cacheKey, result, TimeSpan.FromSeconds(10));

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Contains("formato"))
                {
                    return BadRequest(new ErrorResponse { Error = ex.Message, Code = "FORMATO_DATA_INVALIDO" });
                }
                return NotFound(new ErrorResponse { Error = ex.Message, Code = "CONTA_NAO_ENCONTRADA" });
            }
        }
    }
}


