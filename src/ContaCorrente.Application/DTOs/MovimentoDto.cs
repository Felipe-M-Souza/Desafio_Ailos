using System.ComponentModel.DataAnnotations;

namespace ContaCorrente.Application.DTOs
{
    /// <summary>
    /// Dados para lançamento de movimento na conta
    /// </summary>
    public record LancarMovimentoRequest
    {
        /// <summary>
        /// Data do movimento no formato DD/MM/AAAA
        /// </summary>
        /// <example>27/09/2025</example>
        [Required(ErrorMessage = "A data é obrigatória")]
        [RegularExpression(@"^\d{2}/\d{2}/\d{4}$", ErrorMessage = "Data deve estar no formato DD/MM/AAAA")]
        public string Data { get; init; } = string.Empty;

        /// <summary>
        /// Tipo do movimento (C = Crédito, D = Débito)
        /// </summary>
        /// <example>C</example>
        [Required(ErrorMessage = "O tipo do movimento é obrigatório")]
        [RegularExpression(@"^[CD]$", ErrorMessage = "Tipo deve ser 'C' (Crédito) ou 'D' (Débito)")]
        public char Tipo { get; init; }

        /// <summary>
        /// Valor do movimento (deve ser maior que zero)
        /// </summary>
        /// <example>150.75</example>
        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, 999999.99, ErrorMessage = "O valor deve estar entre R$ 0,01 e R$ 999.999,99")]
        public decimal Valor { get; init; }
    }
    
    /// <summary>
    /// Resposta do lançamento de movimento
    /// </summary>
    /// <param name="IdMovimento">Identificador único do movimento criado</param>
    /// <param name="SaldoAtual">Saldo atual da conta após o movimento</param>
    public record LancarMovimentoResponse(string IdMovimento, decimal SaldoAtual);
    
    /// <summary>
    /// Item do extrato bancário
    /// </summary>
    /// <param name="Data">Data do movimento</param>
    /// <param name="Tipo">Tipo do movimento (C = Crédito, D = Débito)</param>
    /// <param name="Valor">Valor do movimento</param>
    /// <param name="Descricao">Descrição do movimento (opcional)</param>
    public record ExtratoItem(string Data, char Tipo, decimal Valor, string? Descricao = null);
    
    /// <summary>
    /// Resposta do extrato bancário
    /// </summary>
    /// <param name="Itens">Lista de movimentos do período</param>
    /// <param name="SaldoNoPeriodo">Saldo no final do período consultado</param>
    /// <param name="Page">Página atual da paginação</param>
    /// <param name="PageSize">Tamanho da página</param>
    /// <param name="Total">Total de registros encontrados</param>
    public record ExtratoResponse(IReadOnlyList<ExtratoItem> Itens, decimal SaldoNoPeriodo, int Page, int PageSize, int Total);
    
    /// <summary>
    /// Resposta da consulta de saldo
    /// </summary>
    /// <param name="SaldoAtual">Saldo atual da conta</param>
    public record SaldoResponse(decimal SaldoAtual);
}