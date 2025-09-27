using Transferencias.Domain.Entities;

namespace Transferencias.Infrastructure.Repositories;

public interface ITransferenciaRepository
{
    Task<Transferencia> CriarAsync(Transferencia transferencia);
    Task<Transferencia?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Transferencia>> ObterPorContaAsync(int numeroConta);
}
