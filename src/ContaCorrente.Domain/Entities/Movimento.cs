using System;

namespace ContaCorrente.Domain.Entities
{
    public class Movimento
    {
        public string IdMovimento { get; set; } = string.Empty;
        public string IdContaCorrente { get; set; } = string.Empty;
        public DateTime DataMovimento { get; set; }
        public char TipoMovimento { get; set; } // 'C' para Crédito, 'D' para Débito
        public decimal Valor { get; set; }
        public string? Descricao { get; set; } // Descrição opcional do movimento

        public Movimento()
        {
            IdMovimento = Guid.NewGuid().ToString();
        }

        public Movimento(string idContaCorrente, DateTime dataMovimento, char tipoMovimento, decimal valor, string? descricao = null) : this()
        {
            IdContaCorrente = idContaCorrente;
            DataMovimento = dataMovimento;
            TipoMovimento = tipoMovimento;
            Valor = valor;
            Descricao = descricao;
        }

        public bool IsCredito => TipoMovimento == 'C';
        public bool IsDebito => TipoMovimento == 'D';

        public decimal ValorComSinal => IsCredito ? Valor : -Valor;

        public static bool IsTipoValido(char tipo)
        {
            return tipo == 'C' || tipo == 'D';
        }

        public static char TipoCredito => 'C';
        public static char TipoDebito => 'D';
    }
}
