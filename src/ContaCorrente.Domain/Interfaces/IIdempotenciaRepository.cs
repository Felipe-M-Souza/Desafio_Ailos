using ContaCorrente.Domain.Entities;
using System.Threading.Tasks;

namespace ContaCorrente.Domain.Interfaces
{
    public interface IIdempotenciaRepository
    {
        Task<Idempotencia?> ObterPorChaveAsync(string chave);
        Task<Idempotencia> CriarAsync(Idempotencia idempotencia);
    }
}

