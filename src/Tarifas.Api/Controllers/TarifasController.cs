using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tarifas.Application.Handlers;

namespace Tarifas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TarifasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TarifasController> _logger;

    public TarifasController(IMediator mediator, ILogger<TarifasController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtém tarifas cobradas para uma conta
    /// </summary>
    /// <param name="idConta">ID da conta</param>
    /// <returns>Lista de tarifas cobradas</returns>
    [HttpGet("conta/{idConta}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ObterTarifasCobradas(Guid idConta)
    {
        try
        {
            var command = new ObterTarifasCobradasCommand { IdConta = idConta };
            var result = await _mediator.Send(command);

            if (result.Sucesso)
            {
                return Ok(result.Tarifas);
            }

            return BadRequest(new { Mensagem = result.Erro });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter tarifas cobradas");
            return StatusCode(500, new { Mensagem = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém todas as tarifas cobradas
    /// </summary>
    /// <returns>Lista de todas as tarifas cobradas</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ObterTodasTarifas()
    {
        try
        {
            var command = new ObterTodasTarifasCommand();
            var result = await _mediator.Send(command);

            if (result.Sucesso)
            {
                return Ok(result.Tarifas);
            }

            return BadRequest(new { Mensagem = result.Erro });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as tarifas");
            return StatusCode(500, new { Mensagem = "Erro interno do servidor" });
        }
    }
}

public class ObterTarifasCobradasCommand : IRequest<ObterTarifasCobradasResponse>
{
    public Guid IdConta { get; set; }
}

public class ObterTarifasCobradasResponse
{
    public bool Sucesso { get; set; }
    public string? Erro { get; set; }
    public IEnumerable<TarifaCobradaDto>? Tarifas { get; set; }
}

public class ObterTodasTarifasCommand : IRequest<ObterTodasTarifasResponse>
{
}

public class ObterTodasTarifasResponse
{
    public bool Sucesso { get; set; }
    public string? Erro { get; set; }
    public IEnumerable<TarifaCobradaDto>? Tarifas { get; set; }
}

public class TarifaCobradaDto
{
    public Guid Id { get; set; }
    public Guid IdConta { get; set; }
    public int NumeroConta { get; set; }
    public Guid IdTarifa { get; set; }
    public string TipoOperacao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataCobranca { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public Guid? IdOperacaoRelacionada { get; set; }
}
