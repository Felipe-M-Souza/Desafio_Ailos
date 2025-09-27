using Confluent.Kafka;
using System.Text.Json;

namespace Tarifas.Infrastructure.Messaging;

public interface IKafkaMessageProducer
{
    Task PublishAsync<T>(string topic, T message);
}

public class KafkaMessageProducer : IKafkaMessageProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaMessageProducer> _logger;

    public KafkaMessageProducer(IConfiguration configuration, ILogger<KafkaMessageProducer> logger)
    {
        _logger = logger;
        
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            Acks = Acks.All,
            Retries = 3,
            EnableIdempotence = true
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, T message)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = json
            };

            var result = await _producer.ProduceAsync(topic, kafkaMessage);
            _logger.LogInformation("Mensagem publicada com sucesso no tópico {Topic}, offset: {Offset}", topic, result.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem no tópico {Topic}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
