using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Constants;
using ContaCorrente.Application.DTOs;
using ContaCorrente.Domain.Events;
using ContaCorrente.Domain.Interfaces;
using ContaCorrente.Domain.Entities;
using MediatR;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace ContaCorrente.Application.Handlers
{
    public class TransferirEntreContasHandler : IRequestHandler<TransferirEntreContasCommand, TransferirEntreContasResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly ContaCorrente.Domain.Interfaces.ITarifaService _tarifaService;

        public TransferirEntreContasHandler(
            IContaCorrenteRepository contaRepository,
            IMovimentoRepository movimentoRepository,
            ContaCorrente.Domain.Interfaces.ITarifaService tarifaService)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
            _tarifaService = tarifaService;
        }

        public async Task<TransferirEntreContasResponse> Handle(TransferirEntreContasCommand request, CancellationToken cancellationToken)
        {
            // Validar dados de entrada
            ValidarDadosTransferencia(request);

            // Buscar conta origem
            var contaOrigem = await _contaRepository.ObterPorIdAsync(request.IdContaOrigem);
            if (contaOrigem == null)
            {
                throw new ArgumentException(ErrorMessages.INVALID_ACCOUNT);
            }

            // Buscar conta destino
            var contaDestino = await _contaRepository.ObterPorNumeroAsync(request.NumeroContaDestino);
            if (contaDestino == null)
            {
                throw new ArgumentException(ErrorMessages.INVALID_ACCOUNT);
            }

            // Verificar se conta origem está ativa
            if (!contaOrigem.PodeRealizarOperacoes())
            {
                throw new InvalidOperationException(ErrorMessages.INACTIVE_ACCOUNT);
            }

            // Verificar se conta destino está ativa
            if (!contaDestino.PodeRealizarOperacoes())
            {
                throw new InvalidOperationException(ErrorMessages.INACTIVE_ACCOUNT);
            }

            // Verificar se não está tentando transferir para a mesma conta
            if (contaOrigem.IdContaCorrente == contaDestino.IdContaCorrente)
            {
                throw new InvalidOperationException("Não é possível transferir para a mesma conta");
            }

            // Verificar saldo da conta origem
            var saldoOrigem = await _movimentoRepository.ObterSaldoAsync(request.IdContaOrigem);
            if (saldoOrigem < request.Valor)
            {
                throw new InvalidOperationException(ErrorMessages.SALDO_INSUFICIENTE);
            }

            // Converter data
            var dataMovimento = ConverterData(request.Data);

            // Criar movimento de débito na conta origem
            var descricaoOrigem = $"Transferência para conta {request.NumeroContaDestino}";
            if (!string.IsNullOrEmpty(request.Descricao))
            {
                descricaoOrigem += $" - {request.Descricao}";
            }
            
            var movimentoOrigem = new Movimento(
                request.IdContaOrigem, 
                dataMovimento, 
                Movimento.TipoDebito, 
                request.Valor,
                descricaoOrigem);
            
            var movimentoOrigemCriado = await _movimentoRepository.CriarAsync(movimentoOrigem);

            // Criar movimento de crédito na conta destino
            var descricaoDestino = $"Transferência de conta {contaOrigem.Numero}";
            if (!string.IsNullOrEmpty(request.Descricao))
            {
                descricaoDestino += $" - {request.Descricao}";
            }
            
            var movimentoDestino = new Movimento(
                contaDestino.IdContaCorrente, 
                dataMovimento, 
                Movimento.TipoCredito, 
                request.Valor,
                descricaoDestino);
            
            var movimentoDestinoCriado = await _movimentoRepository.CriarAsync(movimentoDestino);

            // Cobrar tarifa de transferência
            await _tarifaService.CobrarTarifaAsync(request.IdContaOrigem, Tarifa.TiposOperacao.Transferencia, movimentoOrigemCriado.IdMovimento);

            // Calcular novo saldo da conta origem
            var novoSaldo = await _movimentoRepository.ObterSaldoAsync(request.IdContaOrigem);

            // Publicar evento de transferência realizada
            var evento = new TransferenciaRealizadaEvent
            {
                IdTransferencia = Guid.NewGuid().ToString(),
                IdContaOrigem = contaOrigem.IdContaCorrente,
                NumeroContaOrigem = contaOrigem.Numero,
                IdContaDestino = contaDestino.IdContaCorrente,
                NumeroContaDestino = contaDestino.Numero,
                Valor = request.Valor,
                DataTransferencia = dataMovimento,
                Descricao = request.Descricao ?? "Transferência entre contas",
                IdMovimentoOrigem = movimentoOrigemCriado.IdMovimento,
                IdMovimentoDestino = movimentoDestinoCriado.IdMovimento,
                SaldoContaOrigem = novoSaldo,
                SaldoContaDestino = await _movimentoRepository.ObterSaldoAsync(contaDestino.IdContaCorrente)
            };


            return new TransferirEntreContasResponse(
                movimentoOrigemCriado.IdMovimento, 
                movimentoDestinoCriado.IdMovimento, 
                novoSaldo);
        }

        private static void ValidarDadosTransferencia(TransferirEntreContasCommand request)
        {
            if (request.Valor <= 0)
            {
                throw new ArgumentException(ErrorMessages.INVALID_VALUE);
            }

            if (request.NumeroContaDestino <= 0)
            {
                throw new ArgumentException(ErrorMessages.INVALID_VALUE);
            }
        }

        private static DateTime ConverterData(string data)
        {
            if (!DateTime.TryParseExact(data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataConvertida))
            {
                throw new ArgumentException("Data deve estar no formato DD/MM/YYYY");
            }

            return dataConvertida;
        }
    }
}
