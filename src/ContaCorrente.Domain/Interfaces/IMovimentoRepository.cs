using ContaCorrente.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContaCorrente.Domain.Interfaces
{
    public interface IMovimentoRepository
    {
        Task<Movimento> CriarAsync(Movimento movimento);
        Task<Movimento?> ObterPorIdAsync(string id);
        Task<IEnumerable<Movimento>> ObterPorContaAsync(string idContaCorrente, DateTime? dataInicio = null, DateTime? dataFim = null, int page = 1, int pageSize = 50);
        Task<decimal> ObterSaldoAsync(string idContaCorrente, DateTime? dataReferencia = null);
        Task<int> ContarMovimentosAsync(string idContaCorrente, DateTime? dataInicio = null, DateTime? dataFim = null);
    }
}

