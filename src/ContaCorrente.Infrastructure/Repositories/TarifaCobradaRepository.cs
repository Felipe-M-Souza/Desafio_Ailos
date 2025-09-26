using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using ContaCorrente.Infrastructure.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContaCorrente.Infrastructure.Repositories
{
    public class TarifaCobradaRepository : ITarifaCobradaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TarifaCobradaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<TarifaCobrada> CriarAsync(TarifaCobrada tarifaCobrada)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                INSERT INTO tarifacobrada (idtarifacobrada, idcontacorrente, idtarifa, tipooperacao, valortarifa, descricao, datacobranca, idoperacaorelacionada)
                VALUES (@IdTarifaCobrada, @IdContaCorrente, @IdTarifa, @TipoOperacao, @ValorTarifa, @Descricao, @DataCobranca, @IdOperacaoRelacionada)";

            await connection.ExecuteAsync(sql, new
            {
                tarifaCobrada.IdTarifaCobrada,
                tarifaCobrada.IdContaCorrente,
                tarifaCobrada.IdTarifa,
                tarifaCobrada.TipoOperacao,
                tarifaCobrada.ValorTarifa,
                tarifaCobrada.Descricao,
                DataCobranca = tarifaCobrada.DataCobranca.ToString("yyyy-MM-dd HH:mm:ss"),
                tarifaCobrada.IdOperacaoRelacionada
            });

            return tarifaCobrada;
        }

        public async Task<IEnumerable<TarifaCobrada>> ObterPorContaAsync(string idContaCorrente, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT idtarifacobrada, idcontacorrente, idtarifa, tipooperacao, valortarifa, descricao, datacobranca, idoperacaorelacionada
                FROM tarifacobrada 
                WHERE idcontacorrente = @idContaCorrente";

            var parameters = new Dictionary<string, object>
            {
                { "idContaCorrente", idContaCorrente }
            };

            if (dataInicio.HasValue)
            {
                sql += " AND datacobranca >= @dataInicio";
                parameters["dataInicio"] = dataInicio.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (dataFim.HasValue)
            {
                sql += " AND datacobranca <= @dataFim";
                parameters["dataFim"] = dataFim.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }

            sql += " ORDER BY datacobranca DESC";

            var results = await connection.QueryAsync(sql, parameters);
            
            var tarifasCobradas = new List<TarifaCobrada>();
            foreach (var result in results)
            {
                tarifasCobradas.Add(new TarifaCobrada
                {
                    IdTarifaCobrada = result.idtarifacobrada,
                    IdContaCorrente = result.idcontacorrente,
                    IdTarifa = result.idtarifa,
                    TipoOperacao = result.tipooperacao,
                    ValorTarifa = (decimal)result.valortarifa,
                    Descricao = result.descricao,
                    DataCobranca = DateTime.Parse(result.datacobranca),
                    IdOperacaoRelacionada = result.idoperacaorelacionada
                });
            }

            return tarifasCobradas;
        }

        public async Task<decimal> ObterTotalTarifasPorContaAsync(string idContaCorrente, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT COALESCE(SUM(valortarifa), 0) 
                FROM tarifacobrada 
                WHERE idcontacorrente = @idContaCorrente";

            var parameters = new Dictionary<string, object>
            {
                { "idContaCorrente", idContaCorrente }
            };

            if (dataInicio.HasValue)
            {
                sql += " AND datacobranca >= @dataInicio";
                parameters["dataInicio"] = dataInicio.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (dataFim.HasValue)
            {
                sql += " AND datacobranca <= @dataFim";
                parameters["dataFim"] = dataFim.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }

            var total = await connection.QuerySingleAsync<decimal>(sql, parameters);
            return total;
        }

        public async Task<bool> ExisteTarifaCobradaParaOperacaoAsync(string idOperacaoRelacionada)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT COUNT(1) 
                FROM tarifacobrada 
                WHERE idoperacaorelacionada = @idOperacaoRelacionada";

            var count = await connection.QuerySingleAsync<int>(sql, new { idOperacaoRelacionada });
            return count > 0;
        }
    }
}


