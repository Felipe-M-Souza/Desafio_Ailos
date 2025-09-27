using MediatR;
using ContaCorrente.Application.DTOs;

namespace ContaCorrente.Application.Queries;

public class ObterContaPorNumeroQuery : IRequest<ContaResponse?>
{
    public int Numero { get; set; }

    public ObterContaPorNumeroQuery(int numero)
    {
        Numero = numero;
    }
}
