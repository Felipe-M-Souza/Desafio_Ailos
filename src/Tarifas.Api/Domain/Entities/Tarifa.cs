namespace Tarifas.Domain.Entities;

public class Tarifa
{
    public Guid Id { get; set; }
    public string TipoOperacao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.Now;
}
