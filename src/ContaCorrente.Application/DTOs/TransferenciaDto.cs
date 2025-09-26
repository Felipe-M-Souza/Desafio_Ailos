namespace ContaCorrente.Application.DTOs
{
    public record TransferirEntreContasRequest(
        int NumeroContaDestino,
        decimal Valor,
        string Data,
        string? Descricao = null
    );

    public record TransferirEntreContasResponse(
        string IdMovimentoOrigem,
        string IdMovimentoDestino,
        decimal SaldoAtual
    );

    public record TransferenciaHistoricoResponse(
        string IdTransferencia,
        int NumeroContaDestino,
        decimal Valor,
        DateTime Data,
        string Descricao,
        string Status
    );
}