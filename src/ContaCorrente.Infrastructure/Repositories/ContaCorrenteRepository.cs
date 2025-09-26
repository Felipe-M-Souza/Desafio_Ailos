using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using ContaCorrente.Infrastructure.Data;
using Dapper;
using System;
using System.Threading.Tasks;

namespace ContaCorrente.Infrastructure.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ContaCorrenteRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Conta?> ObterPorIdAsync(string id)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT idcontacorrente, numero, nome, cpf, ativo, senha, salt
                FROM contacorrente 
                WHERE idcontacorrente = @id";

            return await connection.QueryFirstOrDefaultAsync<Conta>(sql, new { id });
        }

        public async Task<Conta?> ObterPorNumeroAsync(int numero)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT idcontacorrente, numero, nome, cpf, ativo, senha, salt
                FROM contacorrente 
                WHERE numero = @numero";

            return await connection.QueryFirstOrDefaultAsync<Conta>(sql, new { numero });
        }

        public async Task<Conta?> ObterPorCpfAsync(string cpf)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT idcontacorrente, numero, nome, cpf, ativo, senha, salt
                FROM contacorrente 
                WHERE cpf = @cpf";

            return await connection.QueryFirstOrDefaultAsync<Conta>(sql, new { cpf });
        }

        public async Task<Conta> CriarAsync(Conta conta)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                INSERT INTO contacorrente (idcontacorrente, numero, nome, cpf, ativo, senha, salt)
                VALUES (@IdContaCorrente, @Numero, @Nome, @Cpf, @Ativo, @Senha, @Salt)";

            await connection.ExecuteAsync(sql, conta);
            return conta;
        }

        public async Task<Conta> AtualizarAsync(Conta conta)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                UPDATE contacorrente 
                SET numero = @Numero, nome = @Nome, cpf = @Cpf, ativo = @Ativo, senha = @Senha, salt = @Salt
                WHERE idcontacorrente = @IdContaCorrente";

            await connection.ExecuteAsync(sql, conta);
            return conta;
        }

        public async Task<bool> ExisteNumeroAsync(int numero)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT COUNT(1) 
                FROM contacorrente 
                WHERE numero = @numero";

            var count = await connection.QuerySingleAsync<int>(sql, new { numero });
            return count > 0;
        }

        public async Task<bool> ExisteCpfAsync(string cpf)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT COUNT(1) 
                FROM contacorrente 
                WHERE cpf = @cpf";

            var count = await connection.QuerySingleAsync<int>(sql, new { cpf });
            return count > 0;
        }
    }
}
