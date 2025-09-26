using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Queries
{
    public record ObterTarifasQuery() : IRequest<IEnumerable<TarifaResponse>>;
}