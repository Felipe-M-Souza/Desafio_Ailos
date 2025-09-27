using MediatR;
using Transferencias.Application.Services;
using Transferencias.Infrastructure.Messaging;

namespace Transferencias.Application.Handlers;

public class TransferirEntreContasHandler : IRequestHandler<TransferirEntreContasCommand, TransferirEntreContasResponse>
{
    private readonly ITransferenciaService _transferenciaService;
    private readonly IContaCorrenteService _contaCorrenteService;
    private readonly IKafkaMessageProducer _kafkaProducer;

    public TransferirEntreContasHandler(
        ITransferenciaService transferenciaService,
        IContaCorrenteService contaCorrenteService,
        IKafkaMessageProducer kafkaProducer)
    {
        _transferenciaService = transferenciaService;
        _contaCorrenteService = contaCorrenteService;
        _kafkaProducer = kafkaProducer;
    }

    public async Task<TransferirEntreContasResponse> Handle(TransferirEntreContasCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar conta de origem
            var contaOrigem = await _contaCorrenteService.ObterContaPorNumero(request.NumeroContaOrigem);
            if (contaOrigem == null)
            {
                return new TransferirEntreContasResponse
                {
                    Sucesso = false,
                    Erro = "Conta de origem não encontrada",
                    TipoErro = "INVALID_ACCOUNT"
                };
            }

            // 2. Validar conta de destino
            var contaDestino = await _contaCorrenteService.ObterContaPorNumero(request.NumeroContaDestino);
            if (contaDestino == null)
            {
                return new TransferirEntreContasResponse
                {
                    Sucesso = false,
                    Erro = "Conta de destino não encontrada",
                    TipoErro = "INVALID_ACCOUNT"
                };
            }

            // 3. Validar se não é a mesma conta
            if (request.NumeroContaOrigem == request.NumeroContaDestino)
            {
                return new TransferirEntreContasResponse
                {
                    Sucesso = false,
                    Erro = "Não é possível transferir para a mesma conta",
                    TipoErro = "INVALID_ACCOUNT"
                };
            }

            // 4. Realizar débito na conta origem
            var debitoResult = await _contaCorrenteService.RealizarDebito(
                request.NumeroContaOrigem, 
                request.Valor, 
                request.Descricao, 
                request.Token);

            if (!debitoResult.Sucesso)
            {
                return new TransferirEntreContasResponse
                {
                    Sucesso = false,
                    Erro = debitoResult.Erro,
                    TipoErro = debitoResult.TipoErro
                };
            }

            try
            {
                // 5. Realizar crédito na conta destino
                var creditoResult = await _contaCorrenteService.RealizarCredito(
                    request.NumeroContaDestino, 
                    request.Valor, 
                    request.Descricao, 
                    request.Token);

                if (!creditoResult.Sucesso)
                {
                    // 6. Estorno em caso de falha no crédito
                    await _contaCorrenteService.RealizarCredito(
                        request.NumeroContaOrigem, 
                        request.Valor, 
                        "Estorno - Falha na transferência", 
                        request.Token);

                    return new TransferirEntreContasResponse
                    {
                        Sucesso = false,
                        Erro = creditoResult.Erro,
                        TipoErro = creditoResult.TipoErro
                    };
                }

                // 7. Salvar transferência
                var transferencia = await _transferenciaService.CriarTransferencia(
                    request.NumeroContaOrigem,
                    request.NumeroContaDestino,
                    request.Valor,
                    request.Descricao,
                    debitoResult.IdMovimento,
                    creditoResult.IdMovimento);

                // 8. Publicar evento no Kafka
                var evento = new TransferenciaRealizadaEvent
                {
                    IdTransferencia = transferencia.Id,
                    IdContaOrigem = contaOrigem.Id,
                    NumeroContaOrigem = request.NumeroContaOrigem,
                    IdContaDestino = contaDestino.Id,
                    NumeroContaDestino = request.NumeroContaDestino,
                    Valor = request.Valor,
                    DataTransferencia = DateTime.Now,
                    Descricao = request.Descricao,
                    IdMovimentoOrigem = debitoResult.IdMovimento,
                    IdMovimentoDestino = creditoResult.IdMovimento,
                    SaldoContaOrigem = debitoResult.SaldoAtual,
                    SaldoContaDestino = creditoResult.SaldoAtual
                };

                await _kafkaProducer.PublishAsync("transferencias.efetuadas", evento);

                return new TransferirEntreContasResponse
                {
                    Sucesso = true,
                    IdTransferencia = transferencia.Id
                };
            }
            catch (Exception ex)
            {
                // Estorno em caso de exceção
                await _contaCorrenteService.RealizarCredito(
                    request.NumeroContaOrigem, 
                    request.Valor, 
                    "Estorno - Erro na transferência", 
                    request.Token);

                throw;
            }
        }
        catch (Exception ex)
        {
            return new TransferirEntreContasResponse
            {
                Sucesso = false,
                Erro = ex.Message,
                TipoErro = "INTERNAL_ERROR"
            };
        }
    }
}

public class TransferirEntreContasCommand : IRequest<TransferirEntreContasResponse>
{
    public int NumeroContaOrigem { get; set; }
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class TransferirEntreContasResponse
{
    public bool Sucesso { get; set; }
    public string? Erro { get; set; }
    public string? TipoErro { get; set; }
    public Guid? IdTransferencia { get; set; }
}

public class TransferenciaRealizadaEvent
{
    public Guid IdTransferencia { get; set; }
    public Guid IdContaOrigem { get; set; }
    public int NumeroContaOrigem { get; set; }
    public Guid IdContaDestino { get; set; }
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataTransferencia { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public Guid IdMovimentoOrigem { get; set; }
    public Guid IdMovimentoDestino { get; set; }
    public decimal SaldoContaOrigem { get; set; }
    public decimal SaldoContaDestino { get; set; }
}
