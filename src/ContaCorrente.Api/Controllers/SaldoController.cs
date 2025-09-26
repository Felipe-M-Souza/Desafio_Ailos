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
    [Route("api/contas/{id}/saldo")]
    [Authorize]
    [Produces("application/json")]
    public class SaldoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;

        public SaldoController(IMediator mediator, IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        /// <summary>
        /// Obtém o saldo atual da conta
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <returns>Saldo atual da conta</returns>
        /// <response code="200">Saldo obtido com sucesso</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Conta não encontrada</response>
        [HttpGet]
        [ProducesResponseType(typeof(SaldoResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<ActionResult<SaldoResponse>> ObterSaldo(string id)
        {
            try
            {
                // Verificar cache
                var cacheKey = $"saldo_{id}";
                if (_cache.TryGetValue(cacheKey, out SaldoResponse? cachedSaldo))
                {
                    return Ok(cachedSaldo);
                }

                var query = new ObterSaldoQuery(id);
                var result = await _mediator.Send(query);

                // Cache por 10 segundos
                _cache.Set(cacheKey, result, TimeSpan.FromSeconds(10));

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ErrorResponse { Error = ex.Message, Code = "CONTA_NAO_ENCONTRADA" });
            }
        }
    }
}


