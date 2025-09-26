using System;

namespace ContaCorrente.Domain.Entities
{
    public class Tarifa
    {
        public string IdTarifa { get; set; } = string.Empty;
        public string TipoOperacao { get; set; } = string.Empty; // "TRANSFERENCIA", "SAQUE", "DEPOSITO", etc.
        public decimal Valor { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public bool Ativa { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public Tarifa()
        {
            IdTarifa = Guid.NewGuid().ToString();
        }

        public Tarifa(string tipoOperacao, decimal valor, string descricao) : this()
        {
            TipoOperacao = tipoOperacao;
            Valor = valor;
            Descricao = descricao;
        }

        public static class TiposOperacao
        {
            public const string Transferencia = "TRANSFERENCIA";
            public const string Saque = "SAQUE";
            public const string Deposito = "DEPOSITO";
            public const string ConsultaSaldo = "CONSULTA_SALDO";
            public const string Extrato = "EXTRATO";
        }
    }
}


