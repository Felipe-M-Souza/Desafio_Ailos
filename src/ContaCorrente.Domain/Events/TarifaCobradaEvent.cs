namespace ContaCorrente.Domain.Events
{
    public class TarifaCobradaEvent
    {
        public string IdTarifaCobrada { get; set; } = string.Empty;
        public string IdConta { get; set; } = string.Empty;
        public int NumeroConta { get; set; }
        public string IdTarifa { get; set; } = string.Empty;
        public string TipoOperacao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataCobranca { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? IdOperacaoRelacionada { get; set; }
    }
}
