using ContaCorrente.Domain.Entities;
using System.Threading.Tasks;

namespace ContaCorrente.Domain.Interfaces
{
    public interface IContaCorrenteRepository
    {
        Task<Conta?> ObterPorIdAsync(string id);
        Task<Conta?> ObterPorNumeroAsync(int numero);
        Task<Conta?> ObterPorCpfAsync(string cpf);
        Task<Conta> CriarAsync(Conta conta);
        Task<Conta> AtualizarAsync(Conta conta);
        Task<bool> ExisteNumeroAsync(int numero);
        Task<bool> ExisteCpfAsync(string cpf);
    }
}
