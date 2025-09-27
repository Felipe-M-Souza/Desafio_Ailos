using Transferencias.Domain.Entities;
using Transferencias.Infrastructure.Repositories;

namespace Transferencias.Application.Services;

public class TransferenciaService : ITransferenciaService
{
    private readonly ITransferenciaRepository _transferenciaRepository;

    public TransferenciaService(ITransferenciaRepository transferenciaRepository)
    {
        _transferenciaRepository = transferenciaRepository;
    }

    public async Task<Transferencia> CriarTransferencia(
        int numeroContaOrigem,
        int numeroContaDestino,
        decimal valor,
        string descricao,
        Guid idMovimentoOrigem,
        Guid idMovimentoDestino)
    {
        var transferencia = new Transferencia
        {
            Id = Guid.NewGuid(),
            NumeroContaOrigem = numeroContaOrigem,
            NumeroContaDestino = numeroContaDestino,
            Valor = valor,
            Descricao = descricao,
            DataTransferencia = DateTime.Now,
            IdMovimentoOrigem = idMovimentoOrigem,
            IdMovimentoDestino = idMovimentoDestino
        };

        await _transferenciaRepository.CriarAsync(transferencia);
        return transferencia;
    }
}
