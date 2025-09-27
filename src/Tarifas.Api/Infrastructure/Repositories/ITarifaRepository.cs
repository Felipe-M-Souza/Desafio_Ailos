using Tarifas.Domain.Entities;

namespace Tarifas.Infrastructure.Repositories;

public interface ITarifaRepository
{
    Task<Tarifa> CriarAsync(Tarifa tarifa);
    Task<Tarifa?> ObterPorIdAsync(Guid id);
    Task<Tarifa?> ObterPorTipoOperacaoAsync(string tipoOperacao);
    Task<IEnumerable<Tarifa>> ObterTodasAsync();
}
