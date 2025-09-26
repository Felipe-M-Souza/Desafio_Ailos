namespace ContaCorrente.Application.DTOs
{
    public record CriarTarifaRequest(
        string TipoOperacao,
        decimal Valor,
        string Descricao
    );

    public record AtualizarStatusTarifaRequest(
        bool Ativa
    );

    public record TarifaResponse(
        string Id,
        string TipoOperacao,
        decimal Valor,
        string Descricao,
        bool Ativa,
        DateTime DataCriacao
    );

    public record TarifaCobradaResponse(
        string IdTarifaCobrada,
        string IdConta,
        string IdTarifa,
        string TipoOperacao,
        decimal Valor,
        string Descricao,
        DateTime DataCobranca,
        string? IdOperacaoRelacionada
    );
}