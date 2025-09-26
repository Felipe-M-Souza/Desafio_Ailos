using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Queries
{
    public record ObterTarifaQuery(string Id) : IRequest<TarifaResponse?>;
}

