using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ContaCorrente.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Iniciando processamento do comando/query: {RequestName}", requestName);

            try
            {
                var response = await next();
                stopwatch.Stop();

                _logger.LogInformation("Comando/Query {RequestName} processado com sucesso em {ElapsedMilliseconds}ms", 
                    requestName, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, "Erro ao processar comando/query {RequestName} em {ElapsedMilliseconds}ms", 
                    requestName, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}


