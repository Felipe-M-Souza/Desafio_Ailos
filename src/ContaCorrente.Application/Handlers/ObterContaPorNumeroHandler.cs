using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Queries;
using ContaCorrente.Domain.Interfaces;
using MediatR;

namespace ContaCorrente.Application.Handlers;

public class ObterContaPorNumeroHandler : IRequestHandler<ObterContaPorNumeroQuery, ContaResponse?>
{
    private readonly IContaCorrenteRepository _contaRepository;

    public ObterContaPorNumeroHandler(IContaCorrenteRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }

    public async Task<ContaResponse?> Handle(ObterContaPorNumeroQuery request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.ObterPorNumeroAsync(request.Numero);
        
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
