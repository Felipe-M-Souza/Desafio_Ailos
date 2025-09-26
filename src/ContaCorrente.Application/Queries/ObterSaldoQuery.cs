using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Queries
{
    public record ObterSaldoQuery(string IdConta) : IRequest<SaldoResponse>;
}


