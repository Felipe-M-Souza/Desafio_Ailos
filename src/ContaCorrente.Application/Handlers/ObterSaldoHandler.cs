using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Queries;
using ContaCorrente.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContaCorrente.Application.Handlers
{
    public class ObterSaldoHandler : IRequestHandler<ObterSaldoQuery, SaldoResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public ObterSaldoHandler(
            IContaCorrenteRepository contaRepository,
            IMovimentoRepository movimentoRepository)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<SaldoResponse> Handle(ObterSaldoQuery request, CancellationToken cancellationToken)
        {
            // Verificar se conta existe
            var conta = await _contaRepository.ObterPorIdAsync(request.IdConta);
            if (conta == null)
            {
                throw new ArgumentException("Conta não encontrada");
            }

            // Obter saldo atual
            var saldo = await _movimentoRepository.ObterSaldoAsync(request.IdConta);

            return new SaldoResponse(saldo);
        }
    }
}


