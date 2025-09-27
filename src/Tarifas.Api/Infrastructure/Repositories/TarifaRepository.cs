using Dapper;
using Tarifas.Domain.Entities;
using Tarifas.Infrastructure.Data;

namespace Tarifas.Infrastructure.Repositories;

public class TarifaRepository : ITarifaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TarifaRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Tarifa> CriarAsync(Tarifa tarifa)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            INSERT INTO Tarifas (Id, TipoOperacao, Valor, Descricao, Ativo, DataCriacao)
            VALUES (@Id, @TipoOperacao, @Valor, @Descricao, @Ativo, @DataCriacao)";

        await connection.ExecuteAsync(sql, tarifa);
        return tarifa;
    }

    public async Task<Tarifa?> ObterPorIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT * FROM Tarifas WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Tarifa>(sql, new { Id = id });
    }

    public async Task<Tarifa?> ObterPorTipoOperacaoAsync(string tipoOperacao)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT * FROM Tarifas WHERE TipoOperacao = @TipoOperacao AND Ativo = 1";
        return await connection.QueryFirstOrDefaultAsync<Tarifa>(sql, new { TipoOperacao = tipoOperacao });
    }

    public async Task<IEnumerable<Tarifa>> ObterTodasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT * FROM Tarifas WHERE Ativo = 1 ORDER BY DataCriacao DESC";
        return await connection.QueryAsync<Tarifa>(sql);
    }
}
