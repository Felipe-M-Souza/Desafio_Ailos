namespace ContaCorrente.Domain.Events
{
    public class TransferenciaRealizadaEvent
    {
        public string IdTransferencia { get; set; } = string.Empty;
        public string IdContaOrigem { get; set; } = string.Empty;
        public int NumeroContaOrigem { get; set; }
        public string IdContaDestino { get; set; } = string.Empty;
        public int NumeroContaDestino { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataTransferencia { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string IdMovimentoOrigem { get; set; } = string.Empty;
        public string IdMovimentoDestino { get; set; } = string.Empty;
        public decimal SaldoContaOrigem { get; set; }
        public decimal SaldoContaDestino { get; set; }
    }
}
