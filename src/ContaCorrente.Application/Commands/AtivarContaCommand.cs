using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Commands
{
    public record AtivarContaCommand(string IdConta, bool Ativo) : IRequest<ContaResponse>;
}

