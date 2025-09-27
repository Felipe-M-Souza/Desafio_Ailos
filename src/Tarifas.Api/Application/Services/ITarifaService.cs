using Tarifas.Domain.Entities;

namespace Tarifas.Application.Services;

public interface ITarifaService
{
    Task<TarifaCobrada> ProcessarTarifaAsync(TransferenciaRealizadaEvent evento);
    Task<IEnumerable<TarifaCobrada>> ObterTarifasCobradasAsync(Guid idConta);
    Task<decimal> ObterValorTarifaAsync(string tipoOperacao);
}
