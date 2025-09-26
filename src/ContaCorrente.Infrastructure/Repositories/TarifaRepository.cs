using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using ContaCorrente.Infrastructure.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContaCorrente.Infrastructure.Repositories
{
    public class TarifaRepository : ITarifaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TarifaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Tarifa?> ObterPorIdAsync(string id)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT idtarifa, tipooperacao, valor, descricao, ativa, datacriacao
                FROM tarifa 
                WHERE idtarifa = @id";

            var result = await connection.QueryFirstOrDefaultAsync(sql, new { id });
            if (result == null) return null;

            return new Tarifa
            {
                IdTarifa = result.idtarifa,
                TipoOperacao = result.tipooperacao,
                Valor = (decimal)result.valor,
                Descricao = result.descricao,
                Ativa = Convert.ToBoolean(result.ativa),
                DataCriacao = DateTime.Parse(result.datacriacao)
            };
        }

        public async Task<Tarifa?> ObterPorTipoOperacaoAsync(string tipoOperacao)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT idtarifa, tipooperacao, valor, descricao, ativa, datacriacao
                FROM tarifa 
                WHERE tipooperacao = @tipoOperacao AND ativa = 1";

            var result = await connection.QueryFirstOrDefaultAsync(sql, new { tipoOperacao });
            if (result == null) return null;

            return new Tarifa
            {
                IdTarifa = result.idtarifa,
                TipoOperacao = result.tipooperacao,
                Valor = (decimal)result.valor,
                Descricao = result.descricao,
                Ativa = Convert.ToBoolean(result.ativa),
                DataCriacao = DateTime.Parse(result.datacriacao)
            };
        }

        public async Task<IEnumerable<Tarifa>> ObterTodasAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT idtarifa, tipooperacao, valor, descricao, ativa, datacriacao
                FROM tarifa 
                ORDER BY tipooperacao";

            var results = await connection.QueryAsync(sql);
            
            var tarifas = new List<Tarifa>();
            foreach (var result in results)
            {
                tarifas.Add(new Tarifa
                {
                    IdTarifa = result.idtarifa,
                    TipoOperacao = result.tipooperacao,
                    Valor = (decimal)result.valor,
                    Descricao = result.descricao,
                    Ativa = Convert.ToBoolean(result.ativa),
                    DataCriacao = DateTime.Parse(result.datacriacao)
                });
            }

            return tarifas;
        }

        public async Task<Tarifa> CriarAsync(Tarifa tarifa)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                INSERT INTO tarifa (idtarifa, tipooperacao, valor, descricao, ativa, datacriacao)
                VALUES (@IdTarifa, @TipoOperacao, @Valor, @Descricao, @Ativa, @DataCriacao)";

            await connection.ExecuteAsync(sql, new
            {
                tarifa.IdTarifa,
                tarifa.TipoOperacao,
                tarifa.Valor,
                tarifa.Descricao,
                Ativa = tarifa.Ativa ? 1 : 0,
                DataCriacao = tarifa.DataCriacao.ToString("yyyy-MM-dd HH:mm:ss")
            });

            return tarifa;
        }

        public async Task<Tarifa> AtualizarAsync(Tarifa tarifa)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                UPDATE tarifa 
                SET valor = @Valor, descricao = @Descricao, ativa = @Ativa
                WHERE idtarifa = @IdTarifa";

            await connection.ExecuteAsync(sql, new
            {
                tarifa.IdTarifa,
                tarifa.Valor,
                tarifa.Descricao,
                Ativa = tarifa.Ativa ? 1 : 0
            });

            return tarifa;
        }

        public async Task<bool> ExisteTarifaParaTipoAsync(string tipoOperacao)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT COUNT(1) 
                FROM tarifa 
                WHERE tipooperacao = @tipoOperacao";

            var count = await connection.QuerySingleAsync<int>(sql, new { tipoOperacao });
            return count > 0;
        }
    }
}


