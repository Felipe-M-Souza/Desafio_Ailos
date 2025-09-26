using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using ContaCorrente.Infrastructure.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContaCorrente.Infrastructure.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MovimentoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Movimento> CriarAsync(Movimento movimento)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor, descricao)
                VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor, @Descricao)";

            await connection.ExecuteAsync(sql, new
            {
                movimento.IdMovimento,
                movimento.IdContaCorrente,
                DataMovimento = movimento.DataMovimento.ToString("yyyy-MM-dd"),
                movimento.TipoMovimento,
                movimento.Valor,
                movimento.Descricao
            });

            return movimento;
        }

        public async Task<Movimento?> ObterPorIdAsync(string id)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT idmovimento, idcontacorrente, datamovimento, tipomovimento, valor, descricao
                FROM movimento 
                WHERE idmovimento = @id";

            var result = await connection.QueryFirstOrDefaultAsync(sql, new { id });
            if (result == null) return null;

            return new Movimento
            {
                IdMovimento = result.idmovimento,
                IdContaCorrente = result.idcontacorrente,
                DataMovimento = ParseDataMovimento(result.datamovimento),
                TipoMovimento = result.tipomovimento[0], // Converter string para char
                Valor = (decimal)result.valor, // Converter double para decimal
                Descricao = result.descricao
            };
        }

        public async Task<IEnumerable<Movimento>> ObterPorContaAsync(
            string idContaCorrente, 
            DateTime? dataInicio = null, 
            DateTime? dataFim = null, 
            int page = 1, 
            int pageSize = 50)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT idmovimento, idcontacorrente, datamovimento, tipomovimento, valor, descricao
                FROM movimento 
                WHERE idcontacorrente = @idContaCorrente";

            var parameters = new Dictionary<string, object>
            {
                { "idContaCorrente", idContaCorrente }
            };

            if (dataInicio.HasValue)
            {
                sql += " AND datamovimento >= @dataInicio";
                parameters["dataInicio"] = dataInicio.Value.ToString("yyyy-MM-dd");
                Console.WriteLine($"🔍 Filtro data início: {dataInicio.Value:yyyy-MM-dd}");
            }

            if (dataFim.HasValue)
            {
                sql += " AND datamovimento <= @dataFim";
                parameters["dataFim"] = dataFim.Value.ToString("yyyy-MM-dd");
                Console.WriteLine($"🔍 Filtro data fim: {dataFim.Value:yyyy-MM-dd}");
            }

            sql += " ORDER BY datamovimento DESC, idmovimento DESC LIMIT @pageSize OFFSET @offset";
            
            var offset = (page - 1) * pageSize;
            parameters["pageSize"] = pageSize;
            parameters["offset"] = offset;

            var results = await connection.QueryAsync(sql, parameters);
            
            var movimentos = new List<Movimento>();
            foreach (var result in results)
            {
                movimentos.Add(new Movimento
                {
                    IdMovimento = result.idmovimento,
                    IdContaCorrente = result.idcontacorrente,
                    DataMovimento = ParseDataMovimento(result.datamovimento),
                    TipoMovimento = result.tipomovimento[0], // Converter string para char
                    Valor = (decimal)result.valor, // Converter double para decimal
                    Descricao = result.descricao
                });
            }

            return movimentos;
        }

        public async Task<decimal> ObterSaldoAsync(string idContaCorrente, DateTime? dataReferencia = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT 
                    COALESCE(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE 0 END), 0) -
                    COALESCE(SUM(CASE WHEN tipomovimento = 'D' THEN valor ELSE 0 END), 0) as saldo
                FROM movimento 
                WHERE idcontacorrente = @idContaCorrente";

            var parameters = new Dictionary<string, object>
            {
                { "idContaCorrente", idContaCorrente }
            };

            if (dataReferencia.HasValue)
            {
                sql += " AND datamovimento <= @dataReferencia";
                parameters["dataReferencia"] = dataReferencia.Value.ToString("yyyy-MM-dd");
            }

            var result = await connection.QuerySingleAsync<decimal>(sql, parameters);
            return result;
        }

        public async Task<int> ContarMovimentosAsync(string idContaCorrente, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "SELECT COUNT(*) FROM movimento WHERE idcontacorrente = @idContaCorrente";
            var parameters = new Dictionary<string, object>
            {
                { "idContaCorrente", idContaCorrente }
            };

            if (dataInicio.HasValue)
            {
                sql += " AND datamovimento >= @dataInicio";
                parameters["dataInicio"] = dataInicio.Value.ToString("yyyy-MM-dd");
                Console.WriteLine($"🔍 Filtro data início: {dataInicio.Value:yyyy-MM-dd}");
            }

            if (dataFim.HasValue)
            {
                sql += " AND datamovimento <= @dataFim";
                parameters["dataFim"] = dataFim.Value.ToString("yyyy-MM-dd");
                Console.WriteLine($"🔍 Filtro data fim: {dataFim.Value:yyyy-MM-dd}");
            }

            return await connection.QuerySingleAsync<int>(sql, parameters);
        }

        private static DateTime ParseDataMovimento(string dataString)
        {
            // Tentar primeiro o formato novo (yyyy-MM-dd)
            if (DateTime.TryParseExact(dataString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var dataNovoFormato))
            {
                return dataNovoFormato;
            }
            
            // Tentar o formato antigo (dd/MM/yyyy)
            if (DateTime.TryParseExact(dataString, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dataAntigoFormato))
            {
                return dataAntigoFormato;
            }
            
            // Fallback para DateTime.Parse
            return DateTime.Parse(dataString);
        }
    }
}
