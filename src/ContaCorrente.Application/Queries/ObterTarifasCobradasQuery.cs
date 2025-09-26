using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Queries
{
    public record ObterTarifasCobradasQuery(string IdConta, string? DataInicio = null, string? DataFim = null) : IRequest<IEnumerable<TarifaCobradaResponse>>;
}