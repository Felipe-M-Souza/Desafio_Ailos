using Dapper;
using Transferencias.Domain.Entities;
using Transferencias.Infrastructure.Data;

namespace Transferencias.Infrastructure.Repositories;

public class TransferenciaRepository : ITransferenciaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TransferenciaRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Transferencia> CriarAsync(Transferencia transferencia)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            INSERT INTO Transferencias (Id, NumeroContaOrigem, NumeroContaDestino, Valor, Descricao, DataTransferencia, IdMovimentoOrigem, IdMovimentoDestino)
            VALUES (@Id, @NumeroContaOrigem, @NumeroContaDestino, @Valor, @Descricao, @DataTransferencia, @IdMovimentoOrigem, @IdMovimentoDestino)";

        await connection.ExecuteAsync(sql, transferencia);
        return transferencia;
    }

    public async Task<Transferencia?> ObterPorIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT * FROM Transferencias WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Transferencia>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Transferencia>> ObterPorContaAsync(int numeroConta)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT * FROM Transferencias 
            WHERE NumeroContaOrigem = @NumeroConta OR NumeroContaDestino = @NumeroConta
            ORDER BY DataTransferencia DESC";
        
        return await connection.QueryAsync<Transferencia>(sql, new { NumeroConta = numeroConta });
    }
}
