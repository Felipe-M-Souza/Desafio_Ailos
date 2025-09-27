using Confluent.Kafka;
using MediatR;
using System.Text.Json;
using Tarifas.Application.Handlers;

namespace Tarifas.Infrastructure.Messaging;

public class TransferenciaConsumerService : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IMediator _mediator;
    private readonly ILogger<TransferenciaConsumerService> _logger;
    private readonly string _topic;

    public TransferenciaConsumerService(
        IConfiguration configuration,
        IMediator mediator,
        ILogger<TransferenciaConsumerService> logger)
    {
        _mediator = mediator;
        _logger = logger;
        _topic = configuration["Kafka:Topics:Transferencias"] ?? "transferencias.efetuadas";

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            GroupId = "tarifas-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
            EnableAutoOffsetStore = true
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando consumer Kafka para tópico: {Topic}", _topic);
        
        _consumer.Subscribe(_topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    
                    if (result?.Message?.Value != null)
                    {
                        await ProcessarMensagem(result.Message.Value);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Erro ao consumir mensagem do Kafka");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado no consumer Kafka");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer Kafka cancelado");
        }
        finally
        {
            _consumer.Close();
        }
    }

    private async Task ProcessarMensagem(string mensagem)
    {
        try
        {
            _logger.LogInformation("Processando mensagem de transferência: {Mensagem}", mensagem);

            var evento = JsonSerializer.Deserialize<TransferenciaRealizadaEvent>(mensagem);
            if (evento == null)
            {
                _logger.LogWarning("Não foi possível deserializar evento de transferência");
                return;
            }

            var command = new ProcessarTarifaCommand
            {
                IdTransferencia = evento.IdTransferencia,
                Evento = evento
            };

            var result = await _mediator.Send(command);
            
            if (result.Sucesso)
            {
                _logger.LogInformation("Tarifa processada com sucesso: {IdTarifaCobrada}", result.IdTarifaCobrada);
            }
            else
            {
                _logger.LogError("Erro ao processar tarifa: {Erro}", result.Erro);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar mensagem de transferência");
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
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
