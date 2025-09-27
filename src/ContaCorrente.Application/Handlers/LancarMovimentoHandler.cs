using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Constants;
using ContaCorrente.Application.DTOs;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Events;
using ContaCorrente.Domain.Interfaces;
using ContaCorrente.Infrastructure.Services;
using MediatR;

namespace ContaCorrente.Application.Handlers
{
    public class LancarMovimentoHandler
        : IRequestHandler<LancarMovimentoCommand, LancarMovimentoResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly ContaCorrente.Domain.Interfaces.ITarifaService _tarifaService;
        private readonly IEventPublisher _eventPublisher;

        public LancarMovimentoHandler(
            IContaCorrenteRepository contaRepository,
            IMovimentoRepository movimentoRepository,
            ContaCorrente.Domain.Interfaces.ITarifaService tarifaService,
            IEventPublisher eventPublisher
        )
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
            _tarifaService = tarifaService;
            _eventPublisher = eventPublisher;
        }

        public async Task<LancarMovimentoResponse> Handle(
            LancarMovimentoCommand request,
            CancellationToken cancellationToken
        )
        {
            ValidateMovementData(request);

            var account = await GetAccountById(request.IdConta);
            ValidateAccountIsActive(account);

            var movementDate = ParseMovementDate(request.Data);
            await ValidateSufficientBalanceForDebit(request);

            return await ProcessMovement(request, account, movementDate);
        }

        private void ValidateMovementData(LancarMovimentoCommand request)
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

        private async Task<ContaCorrente.Domain.Entities.Conta> GetAccountById(string accountId)
        {
            var account = await _contaRepository.ObterPorIdAsync(accountId);
            if (account == null)
            {
                throw new ArgumentException(ErrorMessages.INVALID_ACCOUNT);
            }
            return account;
        }

        private void ValidateAccountIsActive(ContaCorrente.Domain.Entities.Conta account)
        {
            if (!account.PodeRealizarOperacoes())
            {
                throw new InvalidOperationException(ErrorMessages.INACTIVE_ACCOUNT);
            }
        }

        private async Task ValidateSufficientBalanceForDebit(LancarMovimentoCommand request)
        {
            if (request.Tipo == Movimento.TipoDebito)
            {
                var currentBalance = await _movimentoRepository.ObterSaldoAsync(request.IdConta);
                if (currentBalance < request.Valor)
                {
                    throw new InvalidOperationException(ErrorMessages.SALDO_INSUFICIENTE);
                }
            }
        }

        private async Task<LancarMovimentoResponse> ProcessMovement(
            LancarMovimentoCommand request,
            ContaCorrente.Domain.Entities.Conta account,
            DateTime movementDate
        )
        {
            var createdMovement = await CreateMovement(request, movementDate);
            await ChargeFeeIfApplicable(request, createdMovement.IdMovimento);
            var newBalance = await CalculateNewBalance(request.IdConta);
            await PublishMovementCompletedEvent(
                request,
                account,
                movementDate,
                createdMovement,
                newBalance
            );

            return new LancarMovimentoResponse(createdMovement.IdMovimento, newBalance);
        }

        private async Task<Movimento> CreateMovement(
            LancarMovimentoCommand request,
            DateTime movementDate
        )
        {
            var movement = new Movimento(
                request.IdConta,
                movementDate,
                request.Tipo,
                request.Valor
            );
            return await _movimentoRepository.CriarAsync(movement);
        }

        private async Task ChargeFeeIfApplicable(LancarMovimentoCommand request, string movementId)
        {
            if (request.Tipo == Movimento.TipoDebito)
            {
                await _tarifaService.CobrarTarifaAsync(
                    request.IdConta,
                    Tarifa.TiposOperacao.Saque,
                    movementId
                );
            }
        }

        private async Task<decimal> CalculateNewBalance(string accountId)
        {
            return await _movimentoRepository.ObterSaldoAsync(accountId);
        }

        private async Task PublishMovementCompletedEvent(
            LancarMovimentoCommand request,
            ContaCorrente.Domain.Entities.Conta account,
            DateTime movementDate,
            Movimento createdMovement,
            decimal newBalance
        )
        {
            var movementEvent = new MovimentoRealizadoEvent
            {
                IdMovimento = createdMovement.IdMovimento,
                IdConta = account.IdContaCorrente,
                NumeroConta = account.Numero,
                Tipo = request.Tipo.ToString(),
                Valor = request.Valor,
                DataMovimento = movementDate,
                SaldoAtual = newBalance,
                Descricao = $"Movimento {request.Tipo} - {request.Valor:C}",
            };

            await _eventPublisher.PublishMovimentoRealizadoAsync(movementEvent);
        }

        private static DateTime ParseMovementDate(string dateString)
        {
            if (
                !DateTime.TryParseExact(
                    dateString,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedDate
                )
            )
            {
                throw new ArgumentException("Data deve estar no formato DD/MM/YYYY");
            }

            return parsedDate;
        }
    }
}
