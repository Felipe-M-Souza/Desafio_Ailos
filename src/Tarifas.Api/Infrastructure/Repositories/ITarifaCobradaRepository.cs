using Tarifas.Domain.Entities;

namespace Tarifas.Infrastructure.Repositories;

public interface ITarifaCobradaRepository
{
    Task<TarifaCobrada> CriarAsync(TarifaCobrada tarifaCobrada);
    Task<TarifaCobrada?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<TarifaCobrada>> ObterPorContaAsync(Guid idConta);
    Task<IEnumerable<TarifaCobrada>> ObterTodasAsync();
}
