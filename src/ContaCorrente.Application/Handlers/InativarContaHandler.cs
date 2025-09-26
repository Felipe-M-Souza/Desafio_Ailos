using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Constants;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContaCorrente.Application.Handlers
{
    public class InativarContaHandler : IRequestHandler<InativarContaCommand>
    {
        private readonly IContaCorrenteRepository _contaRepository;

        public InativarContaHandler(IContaCorrenteRepository contaRepository)
        {
            _contaRepository = contaRepository;
        }

        public async Task<Unit> Handle(InativarContaCommand request, CancellationToken cancellationToken)
        {
            // Buscar conta
            var conta = await _contaRepository.ObterPorIdAsync(request.IdConta);
            if (conta == null)
            {
                throw new ArgumentException(ErrorMessages.CONTA_NAO_ENCONTRADA);
            }

            // Verificar se conta está ativa
            if (!conta.Ativo)
            {
                throw new InvalidOperationException(ErrorMessages.CONTA_JA_INATIVA);
            }

            // Validar senha
            var senhaValida = BCrypt.Net.BCrypt.Verify(request.Senha, conta.Senha);
            if (!senhaValida)
            {
                throw new UnauthorizedAccessException(ErrorMessages.USER_UNAUTHORIZED);
            }

            // Inativar conta
            conta.Desativar();
            await _contaRepository.AtualizarAsync(conta);
            
            return Unit.Value;
        }
    }
}
