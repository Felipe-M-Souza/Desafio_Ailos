using MediatR;
using Tarifas.Application.Services;

namespace Tarifas.Application.Handlers;

public class ProcessarTarifaHandler : IRequestHandler<ProcessarTarifaCommand, ProcessarTarifaResponse>
{
    private readonly ITarifaService _tarifaService;
    private readonly ILogger<ProcessarTarifaHandler> _logger;

    public ProcessarTarifaHandler(ITarifaService tarifaService, ILogger<ProcessarTarifaHandler> logger)
    {
        _tarifaService = tarifaService;
        _logger = logger;
    }

    public async Task<ProcessarTarifaResponse> Handle(ProcessarTarifaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processando tarifa para transferência: {IdTransferencia}", request.IdTransferencia);

            var tarifaCobrada = await _tarifaService.ProcessarTarifaAsync(request.Evento);

            return new ProcessarTarifaResponse
            {
                Sucesso = true,
                IdTarifaCobrada = tarifaCobrada.Id,
                Valor = tarifaCobrada.Valor
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar tarifa");
            return new ProcessarTarifaResponse
            {
                Sucesso = false,
                Erro = ex.Message
            };
        }
    }
}

public class ProcessarTarifaCommand : IRequest<ProcessarTarifaResponse>
{
    public Guid IdTransferencia { get; set; }
    public TransferenciaRealizadaEvent Evento { get; set; } = new();
}

public class ProcessarTarifaResponse
{
    public bool Sucesso { get; set; }
    public string? Erro { get; set; }
    public Guid? IdTarifaCobrada { get; set; }
    public decimal? Valor { get; set; }
}
