namespace ContaCorrente.Domain.Events
{
    public class MovimentoRealizadoEvent
    {
        public string IdMovimento { get; set; } = string.Empty;
        public string IdConta { get; set; } = string.Empty;
        public int NumeroConta { get; set; }
        public string Tipo { get; set; } = string.Empty; // "C" ou "D"
        public decimal Valor { get; set; }
        public DateTime DataMovimento { get; set; }
        public decimal SaldoAtual { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }
}
