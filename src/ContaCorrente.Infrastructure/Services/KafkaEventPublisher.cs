using ContaCorrente.Domain.Events;
using ContaCorrente.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;

namespace ContaCorrente.Infrastructure.Services
{
    public interface IEventPublisher
    {
        Task PublishMovimentoRealizadoAsync(MovimentoRealizadoEvent evento);
        Task PublishTransferenciaRealizadaAsync(TransferenciaRealizadaEvent evento);
        Task PublishTarifaCobradaAsync(TarifaCobradaEvent evento);
    }

    public class KafkaEventPublisher : IEventPublisher
    {
        private readonly IMessageProducer _messageProducer;
        private readonly ILogger<KafkaEventPublisher> _logger;

        public KafkaEventPublisher(
            IMessageProducer messageProducer,
            ILogger<KafkaEventPublisher> logger
        )
        {
            _messageProducer = messageProducer;
            _logger = logger;
            _logger.LogInformation("KafkaEventPublisher inicializado com sucesso!");
        }

        public async Task PublishMovimentoRealizadoAsync(MovimentoRealizadoEvent evento)
        {
            try
            {
                await _messageProducer.PublishAsync("movimentos.efetuados", evento);
                _logger.LogInformation(
                    "Evento de movimento realizado publicado: {IdMovimento}",
                    evento.IdMovimento
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao publicar evento de movimento realizado: {IdMovimento}",
                    evento.IdMovimento
                );
                throw;
            }
        }

        public async Task PublishTransferenciaRealizadaAsync(TransferenciaRealizadaEvent evento)
        {
            try
            {
                await _messageProducer.PublishAsync("transferencias.efetuadas", evento);
                _logger.LogInformation(
                    "Evento de transferência realizada publicado: {IdTransferencia}",
                    evento.IdTransferencia
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao publicar evento de transferência realizada: {IdTransferencia}",
                    evento.IdTransferencia
                );
                throw;
            }
        }

        public async Task PublishTarifaCobradaAsync(TarifaCobradaEvent evento)
        {
            try
            {
                await _messageProducer.PublishAsync("tarifas.cobradas", evento);
                _logger.LogInformation(
                    "Evento de tarifa cobrada publicado: {IdTarifaCobrada}",
                    evento.IdTarifaCobrada
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao publicar evento de tarifa cobrada: {IdTarifaCobrada}",
                    evento.IdTarifaCobrada
                );
                throw;
            }
        }
    }
}
