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
    public class LancarMovimentoHandler : IRequestHandler<LancarMovimentoCommand, LancarMovimentoResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly ContaCorrente.Domain.Interfaces.ITarifaService _tarifaService;

        public LancarMovimentoHandler(
            IContaCorrenteRepository contaRepository,
            IMovimentoRepository movimentoRepository,
            ContaCorrente.Domain.Interfaces.ITarifaService tarifaService)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
            _tarifaService = tarifaService;
        }

        public async Task<LancarMovimentoResponse> Handle(LancarMovimentoCommand request, CancellationToken cancellationToken)
        {
            // Validar dados de entrada
            ValidarDadosMovimento(request);

            // Buscar conta
            var conta = await _contaRepository.ObterPorIdAsync(request.IdConta);
            if (conta == null)
            {
                throw new ArgumentException(ErrorMessages.INVALID_ACCOUNT);
            }

            // Verificar se conta está ativa
            if (!conta.PodeRealizarOperacoes())
            {
                throw new InvalidOperationException(ErrorMessages.INACTIVE_ACCOUNT);
            }

            // Converter data
            var dataMovimento = ConverterData(request.Data);

            // Verificar saldo para débitos
            if (request.Tipo == Movimento.TipoDebito)
            {
                var saldoAtual = await _movimentoRepository.ObterSaldoAsync(request.IdConta);
                if (saldoAtual < request.Valor)
                {
                    throw new InvalidOperationException(ErrorMessages.SALDO_INSUFICIENTE);
                }
            }

            // Criar movimento
            var movimento = new Movimento(request.IdConta, dataMovimento, request.Tipo, request.Valor);
            var movimentoCriado = await _movimentoRepository.CriarAsync(movimento);

            // Cobrar tarifa se for saque
            if (request.Tipo == Movimento.TipoDebito)
            {
                await _tarifaService.CobrarTarifaAsync(request.IdConta, Tarifa.TiposOperacao.Saque, movimentoCriado.IdMovimento);
            }

            // Calcular novo saldo
            var novoSaldo = await _movimentoRepository.ObterSaldoAsync(request.IdConta);

            // Publicar evento de movimento realizado
            var evento = new MovimentoRealizadoEvent
            {
                IdMovimento = movimentoCriado.IdMovimento,
                IdConta = conta.IdContaCorrente,
                NumeroConta = conta.Numero,
                Tipo = request.Tipo.ToString(),
                Valor = request.Valor,
                DataMovimento = dataMovimento,
                SaldoAtual = novoSaldo,
                Descricao = $"Movimento {request.Tipo} - {request.Valor:C}"
            };


            return new LancarMovimentoResponse(movimentoCriado.IdMovimento, novoSaldo);
        }

        private static void ValidarDadosMovimento(LancarMovimentoCommand request)
        {
            if (request.Valor <= 0)
            {
                throw new ArgumentException(ErrorMessages.INVALID_VALUE);
            }

            if (!Movimento.IsTipoValido(request.Tipo))
            {
                throw new ArgumentException(ErrorMessages.INVALID_TYPE);
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
