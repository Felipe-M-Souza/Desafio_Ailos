using System;
using System.Threading.Tasks;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using ContaCorrente.Infrastructure.Data;
using Dapper;

namespace ContaCorrente.Infrastructure.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public IdempotenciaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Idempotencia?> ObterPorChaveAsync(string chave)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql =
                @"
                SELECT chave_idempotencia, requisicao, resultado, created_at
                FROM idempotencia 
                WHERE chave_idempotencia = @chave";

            var result = await connection.QueryFirstOrDefaultAsync(sql, new { chave });
            if (result == null)
                return null;

            return new Idempotencia
            {
                ChaveIdempotencia = result.chave_idempotencia,
                Requisicao = result.requisicao,
                Resultado = result.resultado,
                CreatedAt = DateTime.Parse(result.created_at),
            };
        }

        public async Task<Idempotencia> CriarAsync(Idempotencia idempotencia)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql =
                @"
                INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado, created_at)
                VALUES (@ChaveIdempotencia, @Requisicao, @Resultado, @CreatedAt)";

            await connection.ExecuteAsync(
                sql,
                new
                {
                    idempotencia.ChaveIdempotencia,
                    idempotencia.Requisicao,
                    idempotencia.Resultado,
                    CreatedAt = idempotencia.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                }
            );

            return idempotencia;
        }
    }
}

