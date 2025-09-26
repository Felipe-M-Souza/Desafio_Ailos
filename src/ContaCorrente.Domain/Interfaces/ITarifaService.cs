using System.Threading.Tasks;

namespace ContaCorrente.Domain.Interfaces
{
    public interface ITarifaService
    {
        Task<bool> CobrarTarifaAsync(string idContaCorrente, string tipoOperacao, string? idOperacaoRelacionada = null);
        Task<decimal> ObterValorTarifaAsync(string tipoOperacao);
    }
}
