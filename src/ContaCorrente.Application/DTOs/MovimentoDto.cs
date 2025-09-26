namespace ContaCorrente.Application.DTOs
{
    public record LancarMovimentoRequest(string Data, char Tipo, decimal Valor);
    
    public record LancarMovimentoResponse(string IdMovimento, decimal SaldoAtual);
    
    public record ExtratoItem(string Data, char Tipo, decimal Valor, string? Descricao = null);
    
    public record ExtratoResponse(IReadOnlyList<ExtratoItem> Itens, decimal SaldoNoPeriodo, int Page, int PageSize, int Total);
    
    public record SaldoResponse(decimal SaldoAtual);
}
