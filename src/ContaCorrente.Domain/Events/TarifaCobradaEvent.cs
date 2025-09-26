namespace ContaCorrente.Domain.Events
{
    public class TarifaCobradaEvent
    {
        public string IdTarifaCobrada { get; set; } = string.Empty;
        public string IdConta { get; set; } = string.Empty;
        public int NumeroConta { get; set; }
        public string IdTarifa { get; set; } = string.Empty;
        public string NomeTarifa { get; set; } = string.Empty;
        public decimal ValorTarifa { get; set; }
        public DateTime DataCobranca { get; set; }
        public string TipoOperacao { get; set; } = string.Empty; // "TRANSFERENCIA", "SAQUE", etc.
        public string Descricao { get; set; } = string.Empty;
    }
}
