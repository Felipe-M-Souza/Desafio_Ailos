namespace Transferencias.Domain.Entities;

public class Transferencia
{
    public Guid Id { get; set; }
    public int NumeroContaOrigem { get; set; }
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataTransferencia { get; set; }
    public Guid IdMovimentoOrigem { get; set; }
    public Guid IdMovimentoDestino { get; set; }
}
