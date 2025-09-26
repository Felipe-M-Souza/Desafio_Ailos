using ContaCorrente.Domain.Entities;

namespace ContaCorrente.Domain.Interfaces
{
    public interface ITarifaCobradaRepository
    {
        Task<TarifaCobrada> CriarAsync(TarifaCobrada tarifaCobrada);
        Task<IEnumerable<TarifaCobrada>> ObterPorContaAsync(string idContaCorrente, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<decimal> ObterTotalTarifasPorContaAsync(string idContaCorrente, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<bool> ExisteTarifaCobradaParaOperacaoAsync(string idOperacaoRelacionada);
    }
}


