using System;

namespace ContaCorrente.Domain.Entities
{
    public class TarifaCobrada
    {
        public string IdTarifaCobrada { get; set; } = string.Empty;
        public string IdContaCorrente { get; set; } = string.Empty;
        public string IdTarifa { get; set; } = string.Empty;
        public string TipoOperacao { get; set; } = string.Empty;
        public decimal ValorTarifa { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataCobranca { get; set; } = DateTime.UtcNow;
        public string? IdOperacaoRelacionada { get; set; } // ID do movimento que gerou a tarifa

        public TarifaCobrada()
        {
            IdTarifaCobrada = Guid.NewGuid().ToString();
        }

        public TarifaCobrada(
            string idContaCorrente,
            string idTarifa,
            string tipoOperacao,
            decimal valorTarifa,
            string descricao,
            string? idOperacaoRelacionada = null
        )
            : this()
        {
            IdContaCorrente = idContaCorrente;
            IdTarifa = idTarifa;
            TipoOperacao = tipoOperacao;
            ValorTarifa = valorTarifa;
            Descricao = descricao;
            IdOperacaoRelacionada = idOperacaoRelacionada;
        }
    }
}
