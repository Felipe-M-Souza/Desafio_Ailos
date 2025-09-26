using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ContaCorrente.Application.Behaviors
{
    public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly ILogger<IdempotencyBehavior<TRequest, TResponse>> _logger;

        public IdempotencyBehavior(
            IIdempotenciaRepository idempotenciaRepository,
            ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
        {
            _idempotenciaRepository = idempotenciaRepository;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Verificar se o request tem chave de idempotência
            var idempotencyKey = ObterIdempotencyKey(request);
            if (string.IsNullOrEmpty(idempotencyKey))
            {
                return await next();
            }

            // Verificar se já existe uma resposta para esta chave
            var idempotenciaExistente = await _idempotenciaRepository.ObterPorChaveAsync(idempotencyKey);
            if (idempotenciaExistente != null)
            {
                _logger.LogInformation("Retornando resposta idempotente para chave: {IdempotencyKey}", idempotencyKey);
                
                var respostaExistente = JsonSerializer.Deserialize<TResponse>(idempotenciaExistente.Resultado);
                return respostaExistente!;
            }

            // Processar request normalmente
            var response = await next();

            // Salvar resposta para idempotência
            var requisicaoJson = JsonSerializer.Serialize(request);
            var respostaJson = JsonSerializer.Serialize(response);
            
            var idempotencia = new Idempotencia(idempotencyKey, requisicaoJson, respostaJson);
            await _idempotenciaRepository.CriarAsync(idempotencia);

            _logger.LogInformation("Resposta salva para idempotência com chave: {IdempotencyKey}", idempotencyKey);

            return response;
        }

        private static string? ObterIdempotencyKey(TRequest request)
        {
            // Esta implementação assume que o request tem uma propriedade IdempotencyKey
            // Em uma implementação real, você poderia usar reflection ou um atributo
            // Para este exemplo, vamos assumir que existe uma interface IIdempotentRequest
            
            if (request is IIdempotentRequest idempotentRequest)
            {
                return idempotentRequest.IdempotencyKey;
            }

            return null;
        }
    }

    public interface IIdempotentRequest
    {
        string? IdempotencyKey { get; }
    }
}


