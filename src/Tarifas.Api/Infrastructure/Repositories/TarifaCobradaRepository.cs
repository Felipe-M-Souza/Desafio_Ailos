using Dapper;
using Tarifas.Domain.Entities;
using Tarifas.Infrastructure.Data;

namespace Tarifas.Infrastructure.Repositories;

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
        
        var sql = @"
            INSERT INTO TarifasCobradas (Id, IdConta, NumeroConta, IdTarifa, TipoOperacao, Valor, DataCobranca, Descricao, IdOperacaoRelacionada)
            VALUES (@Id, @IdConta, @NumeroConta, @IdTarifa, @TipoOperacao, @Valor, @DataCobranca, @Descricao, @IdOperacaoRelacionada)";

        await connection.ExecuteAsync(sql, tarifaCobrada);
        return tarifaCobrada;
    }

    public async Task<TarifaCobrada?> ObterPorIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT * FROM TarifasCobradas WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<TarifaCobrada>(sql, new { Id = id });
    }

    public async Task<IEnumerable<TarifaCobrada>> ObterPorContaAsync(Guid idConta)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT * FROM TarifasCobradas 
            WHERE IdConta = @IdConta
            ORDER BY DataCobranca DESC";
        
        return await connection.QueryAsync<TarifaCobrada>(sql, new { IdConta = idConta });
    }

    public async Task<IEnumerable<TarifaCobrada>> ObterTodasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT * FROM TarifasCobradas ORDER BY DataCobranca DESC";
        return await connection.QueryAsync<TarifaCobrada>(sql);
    }
}
