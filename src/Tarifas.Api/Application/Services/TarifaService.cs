using Tarifas.Domain.Entities;
using Tarifas.Infrastructure.Repositories;
using Tarifas.Infrastructure.Messaging;

namespace Tarifas.Application.Services;

public class TarifaService : ITarifaService
{
    private readonly ITarifaRepository _tarifaRepository;
    private readonly ITarifaCobradaRepository _tarifaCobradaRepository;
    private readonly IKafkaMessageProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TarifaService> _logger;

    public TarifaService(
        ITarifaRepository tarifaRepository,
        ITarifaCobradaRepository tarifaCobradaRepository,
        IKafkaMessageProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<TarifaService> logger)
    {
        _tarifaRepository = tarifaRepository;
        _tarifaCobradaRepository = tarifaCobradaRepository;
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<TarifaCobrada> ProcessarTarifaAsync(TransferenciaRealizadaEvent evento)
    {
        try
        {
            // 1. Obter tarifa para transferência
            var tarifa = await _tarifaRepository.ObterPorTipoOperacaoAsync("TRANSFERENCIA");
            if (tarifa == null)
            {
                // Criar tarifa padrão se não existir
                tarifa = new Tarifa
                {
                    Id = Guid.NewGuid(),
                    TipoOperacao = "TRANSFERENCIA",
                    Valor = _configuration.GetValue<decimal>("Tarifas:ValorTransferencia", 2.50m),
                    Descricao = "Tarifa de transferência entre contas",
                    Ativo = true,
                    DataCriacao = DateTime.Now
                };
                await _tarifaRepository.CriarAsync(tarifa);
            }

            // 2. Criar tarifa cobrada
            var tarifaCobrada = new TarifaCobrada
            {
                Id = Guid.NewGuid(),
                IdConta = evento.IdContaOrigem,
                NumeroConta = evento.NumeroContaOrigem,
                IdTarifa = tarifa.Id,
                TipoOperacao = "TRANSFERENCIA",
                Valor = tarifa.Valor,
                DataCobranca = DateTime.Now,
                Descricao = $"Tarifa cobrada: {tarifa.Descricao}",
                IdOperacaoRelacionada = evento.IdTransferencia
            };

            // 3. Salvar tarifa cobrada
            await _tarifaCobradaRepository.CriarAsync(tarifaCobrada);

            // 4. Publicar evento no Kafka
            var evento = new TarifaCobradaEvent
            {
                IdTarifaCobrada = tarifaCobrada.Id,
                IdConta = tarifaCobrada.IdConta,
                NumeroConta = tarifaCobrada.NumeroConta,
                IdTarifa = tarifaCobrada.IdTarifa,
                TipoOperacao = tarifaCobrada.TipoOperacao,
                Valor = tarifaCobrada.Valor,
                DataCobranca = tarifaCobrada.DataCobranca,
                Descricao = tarifaCobrada.Descricao,
                IdOperacaoRelacionada = tarifaCobrada.IdOperacaoRelacionada
            };

            await _kafkaProducer.PublishAsync("tarifas.cobradas", evento);

            _logger.LogInformation("Tarifa processada com sucesso: {IdTarifaCobrada}", tarifaCobrada.Id);
            return tarifaCobrada;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar tarifa para transferência: {IdTransferencia}", evento.IdTransferencia);
            throw;
        }
    }

    public async Task<IEnumerable<TarifaCobrada>> ObterTarifasCobradasAsync(Guid idConta)
    {
        return await _tarifaCobradaRepository.ObterPorContaAsync(idConta);
    }

    public async Task<decimal> ObterValorTarifaAsync(string tipoOperacao)
    {
        var tarifa = await _tarifaRepository.ObterPorTipoOperacaoAsync(tipoOperacao);
        return tarifa?.Valor ?? _configuration.GetValue<decimal>("Tarifas:ValorTransferencia", 2.50m);
    }
}

public class TransferenciaRealizadaEvent
{
    public Guid IdTransferencia { get; set; }
    public Guid IdContaOrigem { get; set; }
    public int NumeroContaOrigem { get; set; }
    public Guid IdContaDestino { get; set; }
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataTransferencia { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public Guid IdMovimentoOrigem { get; set; }
    public Guid IdMovimentoDestino { get; set; }
    public decimal SaldoContaOrigem { get; set; }
    public decimal SaldoContaDestino { get; set; }
}

public class TarifaCobradaEvent
{
    public Guid IdTarifaCobrada { get; set; }
    public Guid IdConta { get; set; }
    public int NumeroConta { get; set; }
    public Guid IdTarifa { get; set; }
    public string TipoOperacao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataCobranca { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public Guid? IdOperacaoRelacionada { get; set; }
}
