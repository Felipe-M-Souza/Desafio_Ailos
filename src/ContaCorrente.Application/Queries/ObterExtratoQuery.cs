using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Queries
{
    public record ObterExtratoQuery(string IdConta, string? DataInicio = null, string? DataFim = null, int Page = 1, int PageSize = 50) : IRequest<ExtratoResponse>;
}


