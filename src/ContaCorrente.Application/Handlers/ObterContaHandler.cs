using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Queries;
using ContaCorrente.Domain.Interfaces;
using MediatR;

namespace ContaCorrente.Application.Handlers
{
    public class ObterContaHandler : IRequestHandler<ObterContaQuery, ContaResponse?>
    {
        private readonly IContaCorrenteRepository _contaRepository;

        public ObterContaHandler(IContaCorrenteRepository contaRepository)
        {
            _contaRepository = contaRepository;
        }

        public async Task<ContaResponse?> Handle(ObterContaQuery request, CancellationToken cancellationToken)
        {
            var conta = await _contaRepository.ObterPorIdAsync(request.Id);
            
            if (conta == null)
            {
                return null;
            }

            return new ContaResponse(
                conta.IdContaCorrente,
                conta.Numero,
                conta.Nome,
                conta.Ativo
            );
        }
    }
}


