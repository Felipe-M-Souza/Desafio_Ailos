using ContaCorrente.Domain.Entities;

namespace ContaCorrente.Domain.Interfaces
{
    public interface ITarifaRepository
    {
        Task<Tarifa?> ObterPorIdAsync(string id);
        Task<Tarifa?> ObterPorTipoOperacaoAsync(string tipoOperacao);
        Task<IEnumerable<Tarifa>> ObterTodasAsync();
        Task<Tarifa> CriarAsync(Tarifa tarifa);
        Task<Tarifa> AtualizarAsync(Tarifa tarifa);
        Task<bool> ExisteTarifaParaTipoAsync(string tipoOperacao);
    }
}


