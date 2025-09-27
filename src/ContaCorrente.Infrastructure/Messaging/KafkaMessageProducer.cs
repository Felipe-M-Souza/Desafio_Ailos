using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContaCorrente.Infrastructure.Messaging
{
    public interface IMessageProducer
    {
        Task PublishAsync<T>(string topic, T message);
    }

    public class KafkaMessageProducer : IMessageProducer
    {
        // Kafka Configuration Constants
        private const int DEFAULT_MESSAGE_TIMEOUT_MS = 10000;
        private const int DEFAULT_REQUEST_TIMEOUT_MS = 10000;
        private const int DEFAULT_SOCKET_TIMEOUT_MS = 10000;
        private const int MAX_IN_FLIGHT_MESSAGES = 1;
        private const string DEFAULT_BOOTSTRAP_SERVERS = "localhost:9092";
        private const string BACKUP_LOG_FILE = "kafka-messages.log";

        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaMessageProducer> _logger;
        private readonly string _bootstrapServers;

        public KafkaMessageProducer(
            IConfiguration configuration,
            ILogger<KafkaMessageProducer> logger
        )
        {
            _logger = logger;
            _logger.LogInformation("Iniciando construção do KafkaMessageProducer...");
            _bootstrapServers =
                configuration["Kafka:BootstrapServers"] ?? DEFAULT_BOOTSTRAP_SERVERS;

            var config = new ProducerConfig
            {
                BootstrapServers = _bootstrapServers,
                Acks = Acks.All,
                MessageTimeoutMs = DEFAULT_MESSAGE_TIMEOUT_MS,
                RequestTimeoutMs = DEFAULT_REQUEST_TIMEOUT_MS,
                SocketTimeoutMs = DEFAULT_SOCKET_TIMEOUT_MS,
                EnableIdempotence = true,
                MaxInFlight = MAX_IN_FLIGHT_MESSAGES,
            };

            _producer = new ProducerBuilder<string, string>(config)
                .SetErrorHandler(
                    (_, e) => _logger.LogError("Erro do produtor Kafka: {Error}", e.Reason)
                )
                .SetLogHandler((_, m) => _logger.LogDebug("Log Kafka: {Message}", m.Message))
                .Build();

            _logger.LogInformation(
                "KafkaMessageProducer inicializado com servidor: {BootstrapServers}",
                _bootstrapServers
            );

            _logger.LogInformation("KafkaMessageProducer construído com sucesso!");
        }

        public async Task PublishAsync<T>(string topic, T message)
        {
            try
            {
                var messageJson = JsonSerializer.Serialize(
                    message,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                );

                var kafkaMessage = new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = messageJson,
                    Timestamp = Timestamp.Default,
                };

                _logger.LogInformation(
                    "Publicando mensagem no tópico {Topic}: {Message}",
                    topic,
                    messageJson
                );

                var result = await _producer.ProduceAsync(topic, kafkaMessage);

                _logger.LogInformation(
                    "Mensagem publicada com sucesso no tópico {Topic}, offset: {Offset}",
                    result.Topic,
                    result.Offset
                );

                // Salvar em arquivo de backup
                await SaveToBackupFile(topic, messageJson);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao publicar mensagem no tópico {Topic}. Código: {ErrorCode}",
                    topic,
                    ex.Error.Code
                );

                // Salvar em arquivo de backup em caso de erro
                await SaveToBackupFile(topic, JsonSerializer.Serialize(message));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro inesperado ao publicar mensagem no tópico {Topic}",
                    topic
                );

                // Salvar em arquivo de backup em caso de erro
                await SaveToBackupFile(topic, JsonSerializer.Serialize(message));
                throw;
            }
        }

        private async Task SaveToBackupFile(string topic, string message)
        {
            try
            {
                var backupMessage =
                    $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {topic}: {message}{Environment.NewLine}";
                await File.AppendAllTextAsync(BACKUP_LOG_FILE, backupMessage);
                _logger.LogInformation("Mensagem salva em arquivo de backup: {Topic}", topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar mensagem em arquivo de backup");
            }
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}
