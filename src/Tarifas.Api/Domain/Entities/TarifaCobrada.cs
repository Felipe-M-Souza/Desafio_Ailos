namespace Tarifas.Domain.Entities;

public class TarifaCobrada
{
    public Guid Id { get; set; }
    public Guid IdConta { get; set; }
    public int NumeroConta { get; set; }
    public Guid IdTarifa { get; set; }
    public string TipoOperacao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataCobranca { get; set; } = DateTime.Now;
    public string Descricao { get; set; } = string.Empty;
    public Guid? IdOperacaoRelacionada { get; set; }
}
