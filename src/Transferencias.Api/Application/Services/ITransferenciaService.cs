using Transferencias.Domain.Entities;

namespace Transferencias.Application.Services;

public interface ITransferenciaService
{
    Task<Transferencia> CriarTransferencia(
        int numeroContaOrigem,
        int numeroContaDestino,
        decimal valor,
        string descricao,
        Guid idMovimentoOrigem,
        Guid idMovimentoDestino);
}

public interface IContaCorrenteService
{
    Task<ContaCorrenteResponse?> ObterContaPorNumero(int numeroConta);
    Task<MovimentoResponse> RealizarDebito(int numeroConta, decimal valor, string descricao, string token);
    Task<MovimentoResponse> RealizarCredito(int numeroConta, decimal valor, string descricao, string token);
}

public class ContaCorrenteResponse
{
    public Guid Id { get; set; }
    public int Numero { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}

public class MovimentoResponse
{
    public bool Sucesso { get; set; }
    public string? Erro { get; set; }
    public string? TipoErro { get; set; }
    public Guid? IdMovimento { get; set; }
    public decimal? SaldoAtual { get; set; }
}
