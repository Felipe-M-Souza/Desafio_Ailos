using System.Threading.Tasks;

namespace ContaCorrente.Application.Services
{
    public interface ITarifaService
    {
        Task<bool> CobrarTarifaAsync(string idContaCorrente, string tipoOperacao, string? idOperacaoRelacionada = null);
        Task<decimal> ObterValorTarifaAsync(string tipoOperacao);
    }
}


