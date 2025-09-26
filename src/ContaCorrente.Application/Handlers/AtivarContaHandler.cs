using ContaCorrente.Application.Commands;
using ContaCorrente.Application.DTOs;
using ContaCorrente.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContaCorrente.Application.Handlers
{
    public class AtivarContaHandler : IRequestHandler<AtivarContaCommand, ContaResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;

        public AtivarContaHandler(IContaCorrenteRepository contaRepository)
        {
            _contaRepository = contaRepository;
        }

        public async Task<ContaResponse> Handle(AtivarContaCommand request, CancellationToken cancellationToken)
        {
            // Buscar conta
            var conta = await _contaRepository.ObterPorIdAsync(request.IdConta);
            if (conta == null)
            {
                throw new ArgumentException("Conta não encontrada");
            }

            // Ativar ou desativar conta
            if (request.Ativo)
            {
                conta.Ativar();
            }
            else
            {
                conta.Desativar();
            }

            // Salvar alterações
            var contaAtualizada = await _contaRepository.AtualizarAsync(conta);

            return new ContaResponse(
                contaAtualizada.IdContaCorrente,
                contaAtualizada.Numero,
                contaAtualizada.Nome,
                contaAtualizada.Ativo
            );
        }
    }
}


