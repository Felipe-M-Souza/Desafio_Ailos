using System;

namespace ContaCorrente.Domain.Entities
{
    public class Idempotencia
    {
        public string ChaveIdempotencia { get; set; } = string.Empty;
        public string Requisicao { get; set; } = string.Empty;
        public string Resultado { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public Idempotencia()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public Idempotencia(string chaveIdempotencia, string requisicao, string resultado) : this()
        {
            ChaveIdempotencia = chaveIdempotencia;
            Requisicao = requisicao;
            Resultado = resultado;
        }
    }
}

