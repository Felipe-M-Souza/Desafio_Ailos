using MediatR;
using Tarifas.Application.Services;
using Tarifas.Api.Controllers;

namespace Tarifas.Application.Handlers;

public class ObterTarifasCobradasHandler : IRequestHandler<ObterTarifasCobradasCommand, ObterTarifasCobradasResponse>
{
    private readonly ITarifaService _tarifaService;
    private readonly ILogger<ObterTarifasCobradasHandler> _logger;

    public ObterTarifasCobradasHandler(ITarifaService tarifaService, ILogger<ObterTarifasCobradasHandler> logger)
    {
        _tarifaService = tarifaService;
        _logger = logger;
    }

    public async Task<ObterTarifasCobradasResponse> Handle(ObterTarifasCobradasCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obtendo tarifas cobradas para conta: {IdConta}", request.IdConta);

            var tarifas = await _tarifaService.ObterTarifasCobradasAsync(request.IdConta);
            
            var tarifasDto = tarifas.Select(t => new TarifaCobradaDto
            {
                Id = t.Id,
                IdConta = t.IdConta,
                NumeroConta = t.NumeroConta,
                IdTarifa = t.IdTarifa,
                TipoOperacao = t.TipoOperacao,
                Valor = t.Valor,
                DataCobranca = t.DataCobranca,
                Descricao = t.Descricao,
                IdOperacaoRelacionada = t.IdOperacaoRelacionada
            });

            return new ObterTarifasCobradasResponse
            {
                Sucesso = true,
                Tarifas = tarifasDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter tarifas cobradas");
            return new ObterTarifasCobradasResponse
            {
                Sucesso = false,
                Erro = ex.Message
            };
        }
    }
}

public class ObterTodasTarifasHandler : IRequestHandler<ObterTodasTarifasCommand, ObterTodasTarifasResponse>
{
    private readonly ITarifaService _tarifaService;
    private readonly ILogger<ObterTodasTarifasHandler> _logger;

    public ObterTodasTarifasHandler(ITarifaService tarifaService, ILogger<ObterTodasTarifasHandler> logger)
    {
        _tarifaService = tarifaService;
        _logger = logger;
    }

    public async Task<ObterTodasTarifasResponse> Handle(ObterTodasTarifasCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obtendo todas as tarifas cobradas");

            var tarifas = await _tarifaService.ObterTarifasCobradasAsync(Guid.Empty); // Implementar método para todas
            
            var tarifasDto = tarifas.Select(t => new TarifaCobradaDto
            {
                Id = t.Id,
                IdConta = t.IdConta,
                NumeroConta = t.NumeroConta,
                IdTarifa = t.IdTarifa,
                TipoOperacao = t.TipoOperacao,
                Valor = t.Valor,
                DataCobranca = t.DataCobranca,
                Descricao = t.Descricao,
                IdOperacaoRelacionada = t.IdOperacaoRelacionada
            });

            return new ObterTodasTarifasResponse
            {
                Sucesso = true,
                Tarifas = tarifasDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as tarifas");
            return new ObterTodasTarifasResponse
            {
                Sucesso = false,
                Erro = ex.Message
            };
        }
    }
}
